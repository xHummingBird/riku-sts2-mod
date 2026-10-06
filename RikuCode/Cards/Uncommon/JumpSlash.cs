using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;

namespace Riku.RikuCode.Cards.Uncommon;

public class JumpSlash() : RikuCard(
    1,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8, ValueProp.Move),
        new PowerVar<WeakPower>(1m),
        new PowerVar<VulnerablePower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner.Creature;

        /*
         * Check both debuffs before applying either one.
         * This prevents a newly applied debuff from
         * activating the other condition.
         */
        bool targetWasWeak =
            play.Target.HasPower<WeakPower>();

        bool targetWasVulnerable =
            play.Target.HasPower<VulnerablePower>();

        CenterCardCinematic.Start(
            RunManager.Instance.NetService.NetId);

        if (Owner.Character is Character.Riku riku)
        {
            AudioHelper.PlayRandomFinalAttack();

            await riku.DashTo(
                ownerCreature,
                play.Target,
                distance: 300f,
                durationSeconds: 0.3f,
                overrideAnim: "break");

            riku.PlayVfxOnTarget(
                play.Target,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature)
                    ? "hit_dark"
                    : "hit_normal");

            SfxCmd.Play(
                "res://Riku/sfx/riku_swing_heavy.wav");

            await CommonActions
                .CardAttack(this, play.Target)
                .WithHitFx(
                    null,
                    Character.Riku.GetSfxPath(
                        ownerCreature,
                        "hit_hard (1).wav"))
                .Execute(choiceContext);

            await Task.Delay(400);

            await riku.Retreat(
                ownerCreature,
                "break",
                false,
                0.1f);

            riku.PlayAnimation(
                ownerCreature,
                "appear");

            await Task.Delay(100);
        }
        else
        {
            await CommonActions
                .CardAttack(this, play.Target)
                .WithHitFx(
                    null,
                    Character.Riku.GetSfxPath(
                        ownerCreature,
                        "hit_hard (1).wav"))
                .Execute(choiceContext);
        }

        CenterCardCinematic.End(
            RunManager.Instance.NetService.NetId);

        if (targetWasWeak)
        {
            await PowerCmd.Apply<VulnerablePower>(
                choiceContext,
                play.Target,
                DynamicVars.Vulnerable.BaseValue,
                ownerCreature,
                this);
        }

        if (targetWasVulnerable)
        {
            await PowerCmd.Apply<WeakPower>(
                choiceContext,
                play.Target,
                DynamicVars.Weak.BaseValue,
                ownerCreature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Weak.UpgradeValueBy(1);
        DynamicVars.Vulnerable.UpgradeValueBy(1);
    }
}
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
using Riku.RikuCode.Mechanics;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Rare;

public class Sacrifice() : RikuCard(
    1,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy), IDarkSleight
{
    protected override bool ShouldGlowGoldInternal =>
        Owner.HasPower<DarknessPower>() ||
        Owner.HasPower<DarkMasteryPower>();
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<DarknessPower>(),
        RikuStaticHoverTips.DarkSleight
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var target = play.Target;

        bool hasDarkness =
            ownerCreature != null &&
            (
                ownerCreature.HasPower<DarknessPower>() ||
                ownerCreature.HasPower<DarkMasteryPower>()
            );

        bool targetHasWeak =
            target != null &&
            target.HasPower<WeakPower>();

        int hitCount =
            hasDarkness && targetHasWeak
                ? 2
                : 1;
        await CreatureCmd.Damage(choiceContext, base.Owner.Creature, 3, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, play);
        CenterCardCinematic.Start(
            RunManager.Instance.NetService.NetId);

        if (ownerCreature != null &&
            Owner?.Character is Character.Riku riku)
        {
            AudioHelper.PlayRandomFinalAttack();

            await riku.DashTo(
                ownerCreature,
                target,
                distance: 300f,
                durationSeconds: 0.3f,
                overrideAnim: "break");

            riku.PlayVfxOnTarget(
                target,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature)
                    ? "hit_dark"
                    : "hit_normal");

            SfxCmd.Play(
                "res://Riku/sfx/riku_swing_heavy.wav");

            await CommonActions
                .CardAttack(
                    this,
                    target,
                    hitCount: hitCount)
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
                .CardAttack(
                    this,
                    target,
                    hitCount: hitCount)
                .WithHitFx(
                    null,
                    Character.Riku.GetSfxPath(
                        ownerCreature,
                        "hit_hard (1).wav"))
                .Execute(choiceContext);
        }

        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
    }
}
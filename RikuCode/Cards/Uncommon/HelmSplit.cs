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

public class HelmSplit() : RikuCard(2, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(13, ValueProp.Move),
        new PowerVar<VulnerablePower>(1m),
        new PowerVar<WeakPower>(1m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<WeakPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (ownerCreature != null && Owner?.Character is Character.Riku riku)
        {
            AudioHelper.PlayRandomFinalAttack();
            await riku.DashTo(ownerCreature, play.Target, distance: 300f, durationSeconds: 0.3F, overrideAnim: "break");

            riku.PlayVfxOnTarget(
                play.Target,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal"
            );
            SfxCmd.Play("res://Riku/sfx/riku_swing_heavy.wav");
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"))
                .Execute(choiceContext);
            await Task.Delay(400);
            await riku.Retreat(ownerCreature, "break", false, 0.1F);
            riku.PlayAnimation(ownerCreature, "appear");
            await Task.Delay(100);
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"))
                .Execute(choiceContext);

        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);

        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, base.DynamicVars.Vulnerable.BaseValue,
            base.Owner.Creature, this);
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, base.DynamicVars.Weak.BaseValue,
            base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars.Weak.UpgradeValueBy(1);
        DynamicVars.Vulnerable.UpgradeValueBy(1);
    }
}
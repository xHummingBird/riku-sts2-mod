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

namespace Riku.RikuCode.Cards.Common;

public class HeatDive() : RikuCard(
    1,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move),
        new PowerVar<VulnerablePower>(1m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);

        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Riku riku)
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
                "fire"
                );

            SfxCmd.Play("res://Riku/sfx/riku_swing_heavy.wav");
            SfxCmd.Play(Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"));
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Riku/sfx/From_prev/fire_impact (2).wav")
                .Execute(choiceContext);

            await Task.Delay(400);

            await riku.Retreat(ownerCreature, "break", false, 0.1f);
            riku.PlayAnimation(ownerCreature, "appear");

            await Task.Delay(100);
        }
        else
        {
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Riku/sfx/From_prev/fire_impact (2).wav")
                .Execute(choiceContext);
        }

        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Vulnerable.UpgradeValueBy(1);
    }
}
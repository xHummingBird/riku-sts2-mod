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
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Uncommon;

public class MeteorCrash() : RikuCard(
    2,
    CardType.Attack,
    CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15, ValueProp.Move),
        new PowerVar<AmplifyVulnerablePower>(1m),
        new PowerVar<AmplifyWeakPower>(1m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<WeakPower>()
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
        await PowerCmd.Apply<AmplifyVulnerablePower>(choiceContext, play.Target, base.DynamicVars["AmplifyVulnerablePower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<AmplifyWeakPower>(choiceContext, play.Target, base.DynamicVars["AmplifyWeakPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars["AmplifyVulnerablePower"].UpgradeValueBy(1);
        DynamicVars["AmplifyWeakPower"].UpgradeValueBy(1);
    }
}
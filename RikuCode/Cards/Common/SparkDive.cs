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

public class SparkDive() : RikuCard(
    1,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move),
        new PowerVar<WeakPower>(1m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
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
                Character.Riku.IsInDarkness(ownerCreature)
                    ? "hit_dark"
                    : "hit_normal");

            SfxCmd.Play("res://Riku/sfx/riku_swing_heavy.wav");
            SfxCmd.Play(Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"));
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx("vfx/vfx_attack_lightning", "event:/sfx/characters/defect/defect_lightning_passive")
                .Execute(choiceContext);

            await Task.Delay(400);

            await riku.Retreat(ownerCreature, "break", false, 0.1f);
            riku.PlayAnimation(ownerCreature, "appear");

            await Task.Delay(100);
        }
        else
        {
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx("vfx/vfx_attack_lightning", "event:/sfx/characters/defect/defect_lightning_passive")
                .Execute(choiceContext);
        }

        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Weak.UpgradeValueBy(1);
    }
}
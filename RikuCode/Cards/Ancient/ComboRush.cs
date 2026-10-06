using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Powers;
using Riku.RikuCode.Relics;

namespace Riku.RikuCode.Cards.Ancient;

public class ComboRush() : RikuCard(2, CardType.Attack,
    CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(4, ValueProp.Move),
        new RepeatVar(6),
        new DynamicVar("Sp", 6)
    ];
    
    

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        decimal damage = base.DynamicVars.Damage.PreviewValue;
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (ownerCreature != null && Owner?.Character is Character.Riku riku)
        {
            riku.PlayAnimation(ownerCreature, "combo_rush");
            SfxCmd.Play("res://Riku/sounds/transform.wav");
            await Task.Delay(200);
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            await riku.DashTo(ownerCreature, play.Target, distance: 300f, durationSeconds: 0.1F, overrideAnim: "combo_rush");
            riku.PlayVfxOnTarget(play.Target, "res://Riku/scenes/vfx.tscn", Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            await DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Unpowered)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"))
                .Execute(choiceContext);
            await Task.Delay(87);
            riku.PlayVfxOnTarget(play.Target, "res://Riku/scenes/vfx.tscn", Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            SfxCmd.Play("res://Riku/sfx/riku_swing_down.wav");
            await DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Unpowered)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"))
                .Execute(choiceContext);
            AudioHelper.PlayRandomAttack();
            await Task.Delay(120);
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, ownerCreature, "res://Riku/sfx/riku_swing_up.wav", "res://Riku/sfx/normal/hit_soft.wav", Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            await Task.Delay(200);
            SfxCmd.Play("res://Riku/sfx/riku_swing_down.wav");
            riku.PlayVfxOnTarget(play.Target, "res://Riku/scenes/vfx.tscn", Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            await DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Unpowered)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"))
                .Execute(choiceContext);
            await Task.Delay(20);
            AudioHelper.PlayRandomAttack();
            await Task.Delay(200);
            SfxCmd.Play("res://Riku/sfx/riku_swing_down.wav");
            riku.PlayVfxOnTarget(play.Target, "res://Riku/scenes/vfx.tscn", Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            await DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Unpowered)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"))
                .Execute(choiceContext);
            await Task.Delay(20);
            SfxCmd.Play("res://Riku/sfx/riku_swing_down.wav");
            riku.PlayVfxOnTarget(play.Target, "res://Riku/scenes/vfx.tscn", Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            await DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Unpowered)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"))
                .Execute(choiceContext);
            AudioHelper.PlayRandomFinalAttack();
            await Task.Delay(220);
            SfxCmd.Play("res://Riku/sfx/riku_swing_heavy.wav");
            riku.PlayVfxOnTarget(play.Target, "res://Riku/scenes/vfx.tscn", Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            await CommonActions.CardAttack(this, play.Target, hitCount: 1)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (3).wav"))
                .Execute(choiceContext);
            await Task.Delay(267);
            await riku.Retreat(ownerCreature, "retreat_dark");
        }
        else
            await CommonActions.CardAttack(this, play.Target, hitCount: 6)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"))
                .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        RikuSituationRelicBase? relic = Owner.GetRelic<RikuSituationRelicBase>();
            
        if (relic != null)
        {
            relic.GainSituationPoints(DynamicVars["Sp"].IntValue);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
        DynamicVars["Sp"].UpgradeValueBy(2);
    }
}
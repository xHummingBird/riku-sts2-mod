using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Powers;
using Riku.RikuCode.Relics;

namespace Riku.RikuCode.Cards.Basic;

public class FullCombo() : RikuCard(2, CardType.Attack,
    CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(4, ValueProp.Move),
        new RepeatVar(3),
        new DynamicVar("Sp", 4)
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
            AudioHelper.PlayRandomAttack();
            await riku.DashTo(ownerCreature, play.Target, distance: 300f, durationSeconds: 0.2F, overrideAnim: "combo");
            
            riku.PlayVfxOnTarget(play.Target, "res://Riku/scenes/vfx.tscn", Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            SfxCmd.Play("res://Riku/sfx/riku_swing_down.wav");
            await DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Unpowered)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"))
                .Execute(choiceContext);
            
            if (Owner.Creature.HasPower<DarkMasteryPower>())
                await Task.Delay(20);
            else
                await Task.Delay(87);
            AudioHelper.PlayRandomAttack();
            riku.PlayVfxOnTarget(play.Target, "res://Riku/scenes/vfx.tscn", Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            SfxCmd.Play("res://Riku/sfx/riku_swing_down.wav");
            await DamageCmd.Attack(damage).FromCard(this, play).Targeting(play.Target)
                .WithValueProp(ValueProp.Unpowered)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"))
                .Execute(choiceContext);
            
            await Task.Delay(20);
            if (Owner.Creature.HasPower<DarkMasteryPower>())
            {
                AudioHelper.PlayRandomFinalAttack();
                await Task.Delay(200);
            }
            else
            {
                riku.PlayVfxOnTarget(play.Target, "res://Riku/scenes/vfx.tscn","hit_normal");
                RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, ownerCreature,
                    "res://Riku/sfx/riku_swing_up.wav", "res://Riku/sfx/normal/hit_soft.wav");
                await Task.Delay(133);
                AudioHelper.PlayRandomFinalAttack();
                await Task.Delay(267);
            }
            riku.PlayVfxOnTarget(play.Target, "res://Riku/scenes/vfx.tscn", Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            SfxCmd.Play("res://Riku/sfx/riku_swing_heavy.wav");
            await CommonActions.CardAttack(this, play.Target, hitCount: 1)
                .WithHitFx(null, "res://Riku/sfx/riku_hit_hard (1).wav")
                .Execute(choiceContext);
            if (Owner.Creature.HasPower<DarkMasteryPower>())
            {
                await Task.Delay(200);
            }
            await riku.Retreat(ownerCreature);
        }
        else
            await CommonActions.CardAttack(this, play.Target, hitCount: 3)
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
        DynamicVars["Sp"].UpgradeValueBy(1);
    }
}
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Mechanics;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Uncommon;

public class DarkDivide() : RikuCard(0, CardType.Attack,
    CardRarity.Uncommon, TargetType.AllEnemies), IDarkSleight
{
    protected override bool HasEnergyCostX => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(5, ValueProp.Move),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int num = ResolveEnergyXValue();
        
        
        var ownerCreature = Owner?.Creature;
        if (Owner.HasPower<DarkMasteryPower>() || Owner.HasPower<DarknessPower>())
            num += 2;
        
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (ownerCreature != null && Owner?.Character is Character.Riku riku && num > 0)
        {
            SfxCmd.Play("res://Riku/sfx/formchange.wav");
            AudioHelper.PlayRandomFinalAttack();
            var enemies = CombatState.HittableEnemies.ToList();

            if (ownerCreature == null || riku == null || enemies.Count == 0)
                return;
            await riku.DashTo(ownerCreature, enemies[0] , distance: 650f, durationSeconds: 0.2F, overrideAnim: "firaga_dark");
            await Task.Delay((int)(0.067f * 1000f));
            SfxCmd.Play("res://Riku/sfx/dark_firaga.wav");
            SfxCmd.Play("res://Riku/sfx/From_prev/fire_impact (2).wav");
            await Task.Delay((int)(0.133f * 1000f));
            foreach (var enemy in enemies)
                riku.PlayVfxOnTarget(
                    enemy,
                    "res://Riku/scenes/vfx.tscn",
                    "hit_dark");
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(num).FromCard(this, play)
                .TargetingAllOpponents(base.CombatState)
                .Execute(choiceContext);
            await Task.Delay(400);
            await riku.Retreat(ownerCreature, duration: 0.2F);
        }
        else
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(num).FromCard(this, play)
                .TargetingAllOpponents(base.CombatState)
                .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
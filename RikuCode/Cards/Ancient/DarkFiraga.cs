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

namespace Riku.RikuCode.Cards.Ancient;

public class DarkFiraga() : RikuCard(0, CardType.Attack,
    CardRarity.Ancient, TargetType.AllEnemies), ISituationCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(27, ValueProp.Move),
        new DynamicVar("DebuffMultiplier" ,2)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
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
                    "dark_firaga");
            await CommonActions.CardAttack(this, play.Target)
                .Execute(choiceContext);
            await Task.Delay(400);
            await riku.Retreat(ownerCreature, duration: 0.2F);
        }
        else await CommonActions.CardAttack(this, play.Target)
            .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, base.CombatState.HittableEnemies, DynamicVars["DebuffMultiplier"].IntValue, base.Owner.Creature, this);
        await PowerCmd.Apply<WeakPower>(choiceContext, base.CombatState.HittableEnemies, DynamicVars["DebuffMultiplier"].IntValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7m);
    }
}
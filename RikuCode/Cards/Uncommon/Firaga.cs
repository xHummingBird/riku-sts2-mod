using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Mechanics;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Uncommon;

public class Firaga() : RikuCard(3, CardType.Attack,
    CardRarity.Uncommon, TargetType.AllEnemies), IDarkSleight
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(14m),
        new ExtraDamageVar(7m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((card, _) =>
            {
                return card.Owner.HasPower<DarknessPower>() ||
                       card.Owner.HasPower<DarkMasteryPower>()
                    ? 1m
                    : 0m;
            }),
        new EnergyVar(1)
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
            await riku.DashTo(ownerCreature, enemies[0] , distance: 650f, durationSeconds: 0.2F, overrideAnim: "firaga");
            await Task.Delay((int)(0.067f * 1000f));
            SfxCmd.Play("res://Riku/sfx/dark_firaga.wav");
            SfxCmd.Play("res://Riku/sfx/From_prev/fire_impact (2).wav");
            await Task.Delay((int)(0.133f * 1000f));
            foreach (var enemy in enemies)
                riku.PlayVfxOnTarget(
                    enemy,
                    "res://Riku/scenes/vfx.tscn",
                    Character.Riku.IsInDarkness(ownerCreature) ? "dark_firaga" : "fire");
            await CommonActions.CardAttack(this, play.Target)
                .Execute(choiceContext);
            await Task.Delay(400);
            await riku.Retreat(ownerCreature, duration: 0.2F);
        }
        else await CommonActions.CardAttack(this, play.Target)
            .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(4);
        DynamicVars.ExtraDamage.UpgradeValueBy(2);
    }
    
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this)
        {
            return Task.CompletedTask;
        }
        if (base.IsClone)
        {
            return Task.CompletedTask;
        }
        int amount = CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) => e.CardPlay.Card.Type == CardType.Skill && e.CardPlay.Player == base.Owner && e.HappenedThisTurn(base.CombatState));
        ReduceCostBy(amount);
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != base.Owner)
        {
            return Task.CompletedTask;
        }
        if (cardPlay.Card.Type != CardType.Skill)
        {
            return Task.CompletedTask;
        }
        ReduceCostBy(1);
        return Task.CompletedTask;
    }

    private void ReduceCostBy(int amount)
    {
        base.EnergyCost.AddThisTurn(-amount);
    }
}
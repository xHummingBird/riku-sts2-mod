using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Cards.Ancient;
using Riku.RikuCode.Relics;

namespace Riku.RikuCode.Powers;

public class DarkMasteryPower : RikuPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override bool AllowNegative =>
        false;
    
    private IEnumerable<CardModel> GetSituationCommandCards()
    {
        var hand =
            PileType.Hand.GetPile(Owner.Player);

        return hand.Cards
            .OfType<SituationCommand>();
    }

    public override async Task AfterApplied(
        Creature? applier,
        CardModel? cardSource)
    {
        var relic =
            Owner.Player.GetRelic<RikuSituationRelicBase>();

        if (relic != null)
        {
            relic.ClearDarkness();
        }

        if (Owner.HasPower<DarknessPower>())
        {
            await PowerCmd.Remove<DarknessPower>(
                Owner);
        }

        var existingCard = Owner.Player.PlayerCombatState?
            .AllCards
            .OfType<SituationCommand>()
            .FirstOrDefault();

        if (existingCard == null)
        {
            var card =
                CombatState.CreateCard<SituationCommand>(
                    Owner.Player);

            if (Owner.Player.GetRelic<MirageSplit>() != null)
            {
                CardCmd.Upgrade(card);
            }

            await CardPileCmd.AddGeneratedCardToCombat(
                card,
                PileType.Hand,
                Owner.Player);
        }
        else if (existingCard.Pile?.Type != PileType.Hand)
        {
            await CardPileCmd.Add(
                existingCard,
                PileType.Hand);
        }
        
        var ownerCreature = Owner;
        var riku = Owner?.Player.Character as Character.Riku;
        if (ownerCreature != null && riku != null)
        {
            SfxCmd.Play("res://Riku/sounds/transform.wav");
            SfxCmd.Play("res://Riku/sfx/From_prev/from_tifa/limit_break_thunder.wav");
            SfxCmd.Play("res://Riku/sfx/formchange.wav");
            float duration = riku.PlayAnimation(ownerCreature, "transform").total;
            await Task.Delay((int)(1.1f * 1000f));
        }
    }
    
    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        var baseDarkness = 2;
        
        if (dealer != Owner)
            return 0m;

        if (!props.IsPoweredAttack())
            return 0m;

        if (Owner.Player.GetRelic<WayToTheDawn>() != null)
            baseDarkness += 1;

        baseDarkness += Owner.GetPowerAmount<PowerOfDarknessPower>();
        
        return baseDarkness;
    }

    public override decimal ModifyBlockAdditive(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        var baseDarkness = 2;
        
        if (cardSource == null)
            return 0m;

        if (cardSource.Owner.Creature != Owner)
            return 0m;

        if (!props.IsPoweredCardOrMonsterMoveBlock())
            return 0m;
        
        if (Owner.Player.GetRelic<WayToTheDawn>() != null)
            baseDarkness += 2;

        baseDarkness += Owner.GetPowerAmount<PowerOfDarknessPower>();
        
        return baseDarkness;
    }
    
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
            return;

        if (base.Owner.GetPowerAmount<DarkMasteryPower>() <= 1)
        {
            await PowerCmd.Remove(this);
            var ownerCreature = Owner;
            foreach (CardModel card in
                     GetSituationCommandCards().ToList())
            {
                await CardCmd.Exhaust(
                    choiceContext,
                    card);
            }
            var relic =
                Owner.Player.GetRelic<RikuSituationRelicBase>();

            if (relic != null)
            {
                relic.SetSituationPoints(0);
                relic.CommandReady = false;
            }
            if (ownerCreature != null && Owner?.Player?.Character is Character.Riku riku)
                riku.PlayAnimation(ownerCreature, "idle");
        }
        else await PowerCmd.Decrement(this);
    }
    
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != base.Owner.Side)
            return;

        var player = Owner.Player;
        var playerState = player.PlayerCombatState;

        if (playerState == null)
            return;
        
        if (playerState.AllCards.OfType<SituationCommand>().All(c => c.Pile?.Type != PileType.Hand))
        {
            var cards = playerState.AllCards
                .OfType<SituationCommand>()
                .Where(c =>
                    c.Pile == null ||
                    c.Pile.Type != PileType.Hand);

            await CardPileCmd.Add(
                cards,
                PileType.Hand);
        }
    }
}
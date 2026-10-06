using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using Riku.RikuCode.Mechanics;

namespace Riku.RikuCode.Relics;

public class KeybladeOfHeart : RikuRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner.Creature) && base.Owner.PlayerCombatState.TurnNumber <= 1)
        {
            IReadOnlyList<CardModel> readOnlyList = (from c in base.Owner.Character.CardPool.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
                where c is IDarkSleight
                select c).ToList();
            if (readOnlyList.Count > 0)
            {
                Flash();
                List<CardModel> cards = CardFactory.GetDistinctForCombat(base.Owner, readOnlyList, 2, base.Owner.RunState.Rng.CombatCardGeneration).ToList();
                Flash();
                await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, base.Owner);
            }
        }
    }
}
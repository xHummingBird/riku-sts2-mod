using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Riku.RikuCode.Mechanics;

namespace Riku.RikuCode.Cards.Uncommon;

public class SleightOfHand() : RikuCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel cardModel;
        cardModel = CardFactory.GetDistinctForCombat(base.Owner, from c in base.Owner.Character.CardPool.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint)
            where c is IDarkSleight
            select c, 1, base.Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (cardModel != null)
        {
            cardModel.SetToFreeThisTurn();
            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, base.Owner);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Riku.RikuCode.Powers;

public class FervorPower : RikuPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player)
        {
            return;
        }
        CardPile hand = PileType.Hand.GetPile(base.Owner.Player);
        for (int i = 0; i < base.Amount; i++)
        {
            List<CardModel> items = hand.Cards.Where((CardModel c) => c.Type == CardType.Attack && !c.Keywords.Contains(CardKeyword.Unplayable)).ToList();
            CardModel cardModel = base.Owner.Player.RunState.Rng.Shuffle.NextItem(items);
            if (cardModel != null)
            {
                await CardCmd.AutoPlay(choiceContext, cardModel, null);
            }
        }
    }
}
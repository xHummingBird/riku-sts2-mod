using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Riku.RikuCode.Cards.Rare;

public class ComboMaster() : RikuCard(2, CardType.Skill,
    CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        foreach (CardModel card in PileType.Hand.GetPile(base.Owner).Cards)
        {
            if (card.EnergyCost.CostsX)
                continue;
            
            if (card.Type == CardType.Attack || IsUpgraded && card.Type == CardType.Skill)
            {
                card.SetToFreeThisTurn();
            }
        }

        await PowerCmd.Apply<NoDrawPower>(choiceContext, base.Owner.Creature, 1m, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
    }
}
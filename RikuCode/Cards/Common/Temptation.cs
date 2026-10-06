using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Riku.RikuCode.Mechanics;

namespace Riku.RikuCode.Cards.Common;

public class Temptation() : RikuCard(
    1,
    CardType.Skill,
    CardRarity.Common,
    TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var handBefore =
            PileType.Hand
                .GetPile(Owner)
                .Cards
                .ToHashSet();

        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            Owner);

        var hand =
            PileType.Hand.GetPile(Owner);

        var drawnCards =
            hand.Cards
                .Except(handBefore)
                .ToList();

        foreach (var card in drawnCards)
        {
            if (card is not IDarkSleight)
            {
                await CardCmd.Discard(
                    choiceContext,
                    card);
            }
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
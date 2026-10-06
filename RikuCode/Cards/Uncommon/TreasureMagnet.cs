using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Riku.RikuCode.Cards.Uncommon;

public class TreasureMagnet() : RikuCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CardPileCmd.Draw(
            choiceContext,
            2,
            Owner);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner.Creature, 2, Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
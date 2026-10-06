using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Mechanics;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Uncommon;

public class DarkRoll() : RikuCard(0, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self), IDarkSleight
{
    protected override bool ShouldGlowGoldInternal => base.Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>();
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(3, ValueProp.Move),
        new CardsVar(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DarknessPower>(),
        RikuStaticHoverTips.DarkSleight,
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        var hand = PileType.Hand.GetPile(Owner);
        var cardsBeforeDraw = hand.Cards.ToHashSet();

        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.IntValue,
            Owner);

        CardModel? drawnCard = hand.Cards
            .FirstOrDefault(card => !cardsBeforeDraw.Contains(card));

        if (drawnCard is IDarkSleight)
        {
            await CommonActions.CardBlock(this, play);
            if (Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>())
            {
                drawnCard.SetToFreeThisTurn();
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
    }
}

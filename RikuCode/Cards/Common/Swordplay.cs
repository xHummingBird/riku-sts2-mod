using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Riku.RikuCode.Cards.Common;

public class Swordplay() : RikuCard(1, CardType.Skill,
    CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        if (base.IsUpgraded)
        {
            foreach (CardModel item in PileType.Hand.GetPile(base.Owner).Cards.Where((CardModel c) => c.IsUpgradable))
            {
                CardCmd.Upgrade(item);
            }
            return;
        }
        CardModel cardModel = await CardSelectCmd.FromHandForUpgrade(choiceContext, base.Owner, this);
        if (cardModel != null)
        {
            CardCmd.Upgrade(cardModel);
        }
    }
}
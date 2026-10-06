using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Riku.RikuCode.Relics;

namespace Riku.RikuCode.Cards.Rare;

public class Vanish() : RikuCard(3, CardType.Power,
    CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<IntangiblePower>(1m),
        new DynamicVar("Sp", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<IntangiblePower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        IEnumerable<CardModel> enumerable = PileType.Hand.GetPile(base.Owner).Cards.ToList();
        await CardCmd.Discard(choiceContext, enumerable);
        await PowerCmd.Apply<IntangiblePower>(choiceContext, Owner.Creature, DynamicVars["IntangiblePower"].BaseValue,
            Owner.Creature, this);

        RikuSituationRelicBase? relic = Owner.GetRelic<RikuSituationRelicBase>();

        if (relic != null)
        {
            relic.GainSituationPoints(DynamicVars["Sp"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}

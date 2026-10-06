using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Uncommon;

public class DarkInsight() : RikuCard(
    1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DarkInsightPower>(1m),
        new CardsVar(1)
    ];
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<DarkInsightPower>(
            choiceContext,
            base.Owner.Creature,
            DynamicVars["DarkInsightPower"].BaseValue,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DarkInsightPower"].UpgradeValueBy(1);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
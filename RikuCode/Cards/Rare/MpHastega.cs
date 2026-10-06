using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Rare;

public class MpHastega() : RikuCard(
    2,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("MpHastega", 1),
        new EnergyVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<MpHastegaPower>(
            choiceContext,
            base.Owner.Creature,
            DynamicVars["MpHastega"].BaseValue,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MpHastega"].UpgradeValueBy(1);
        DynamicVars.Energy.UpgradeValueBy(1);
    }
}
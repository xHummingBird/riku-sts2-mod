using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Uncommon;

public class PowerOfDarkness() : RikuCard(
    1,
    CardType.Power,
    CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PowerOfDarknessPower>(1m)
    ];
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<PowerOfDarknessPower>(
            choiceContext,
            base.Owner.Creature,
            DynamicVars["PowerOfDarknessPower"].BaseValue,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PowerOfDarknessPower"].UpgradeValueBy(1);
    }
}
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Rare;

public class DarkPresence() : RikuCard(
    1,
    CardType.Power,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DarkPresencePower>(25m)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<DarkPresencePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["DarkPresencePower"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DarkPresencePower"].UpgradeValueBy(25m);
    }
}
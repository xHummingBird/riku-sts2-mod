using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Rare;

public class CriticalCounter() : RikuCard(
    2,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(15, ValueProp.Move),
        new PowerVar<CriticalCounterPower>(1m)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        await PowerCmd.Apply<CriticalCounterPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["CriticalCounterPower"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(5m);
    }
}
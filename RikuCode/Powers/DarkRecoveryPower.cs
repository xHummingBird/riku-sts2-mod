using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Mechanics;

namespace Riku.RikuCode.Powers;

public class DarkRecoveryPower : RikuPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override bool AllowNegative =>
        false;

        public override async Task AfterCardPlayed(
            PlayerChoiceContext choiceContext,
            CardPlay cardPlay)
        {
            if (cardPlay.Card is not IDarkSleight)
                return;

            await CreatureCmd.GainBlock(
                Owner,
                Amount,
                ValueProp.Unpowered,
                null);
        }
}

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Riku.RikuCode.Powers;

public class LingeringWillPower : RikuPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (!(amount == 0m) && power.GetTypeForAmount(amount) == PowerType.Debuff && power.Owner.IsEnemy && applier == base.Owner && !(power is ITemporaryPower))
        {
            Flash();
            await CreatureCmd.Damage(choiceContext, power.Owner, base.Amount, ValueProp.Unpowered, base.Owner);
        }
    }
}
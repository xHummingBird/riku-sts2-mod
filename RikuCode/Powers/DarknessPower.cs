using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Mechanics;

namespace Riku.RikuCode.Powers;

public class DarknessPower : RikuPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override bool AllowNegative =>
        false;

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (dealer != Owner)
            return 0m;

        if (!props.IsPoweredAttack())
            return 0m;

        return Amount;
    }

    public override decimal ModifyBlockAdditive(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource == null)
            return 0m;

        if (cardSource.Owner.Creature != Owner)
            return 0m;

        if (!props.IsPoweredCardOrMonsterMoveBlock())
            return 0m;

        return Amount;
    }
}
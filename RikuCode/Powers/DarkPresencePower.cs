using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Riku.RikuCode.Powers;

public class DarkPresencePower : RikuPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack())
        {
            return 1m;
        }
        if (cardSource == null)
        {
            return 1m;
        }
        if (dealer != base.Owner && !base.Owner.Pets.Contains<Creature>(dealer))
        {
            return 1m;
        }
        if (target == null || !target.HasPower<WeakPower>())
        {
            return 1m;
        }
        return 1m + (decimal)base.Amount / 100m;
    }
}
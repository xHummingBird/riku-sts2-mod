using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Mechanics;

namespace Riku.RikuCode.Relics;


public class Oblivion : RikuRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner.Creature)
            return 1m;

        if (!props.IsPoweredAttack())
            return 1m;

        if (cardSource is not IDarkSleight)
            return 1m;

        return 1.25m;
    }
}
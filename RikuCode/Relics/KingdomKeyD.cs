using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Relics;

public class KingdomKeyD : RikuRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == base.Owner.Creature.Side && combatState.RoundNumber <= 1)
        {
            Flash();
            var situationRelic =
                Owner.GetRelic<RikuSituationRelicBase>();

            if (situationRelic == null)
                return;

            situationRelic?.GainSituationPoints(10);
        }
    }
}
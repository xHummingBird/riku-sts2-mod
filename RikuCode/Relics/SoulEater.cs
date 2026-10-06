using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Riku.RikuCode.Relics;

public class SoulEater : RikuRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == base.Owner.Creature.Side && combatState.RoundNumber <= 1)
        {
            Flash();
            var situationRelic =
                Owner.GetRelic<RikuSituationRelicBase>();

            if (situationRelic == null)
                return;

            await situationRelic.GainDarknessProgress(
                new ThrowingPlayerChoiceContext(),
                1);
        }
    }
}
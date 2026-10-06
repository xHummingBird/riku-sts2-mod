using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Riku.RikuCode.Mechanics;

namespace Riku.RikuCode.Powers;

public class DarkResonancePower : RikuPower
{
    private bool _triggeredThisTurn;

    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override bool AllowNegative =>
        false;

    public override Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side == Owner.Side)
        {
            _triggeredThisTurn = false;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (_triggeredThisTurn)
            return;

        if (cardPlay.Card.Owner?.Creature != Owner)
            return;

        if (cardPlay.Card is not IDarkSleight)
            return;

        _triggeredThisTurn = true;

        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Owner,
            Amount,
            null, null);
    }
}
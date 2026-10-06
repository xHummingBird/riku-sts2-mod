using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Riku.RikuCode.Cards.Uncommon;
using Riku.RikuCode.Mechanics;
using Riku.RikuCode.Potions;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Relics;

public abstract class RikuSituationRelicBase : RikuRelic
{
    private int _situationPoints;

    private bool _isActivatingDarkness;
    private int _darknessProgress;
    private bool _darknessPrimed;

    public override RelicRarity Rarity =>
        RelicRarity.Starter;

    public override bool ShowCounter =>
        CombatManager.Instance.IsInProgress;

    public override int DisplayAmount =>
        SituationPoints;
    
    private int GetDarknessPowerAmount()
    {
        return Owner.GetRelic<WayToTheDawn>() != null
            ? 3
            : 2;
    }

    public int SituationPoints =>
        _situationPoints;
    
    private bool _commandReady;
    
    public bool CommandReady
    {
        get => _commandReady;
        set => CommandReadyInternal = value;
    }

    private bool CommandReadyInternal
    {
        get => _commandReady;

        set
        {
            AssertMutable();

            _commandReady = value;

            UpdateDisplay();
        }
    }

    public int DarknessProgress =>
        _darknessProgress;

    public bool DarknessPrimed =>
        _darknessPrimed;

    public int MaxSituationPoints => 30;

    protected virtual int AttackSpGain => 2;

    protected virtual int SpGainPerTurn => 1;

    protected virtual int DarkSleightSpGain => 3;

    protected virtual int DarknessSpGain => 2;

    protected virtual int DarknessRequired => 3;

    protected virtual int DarkMasteryTurns => 4;

    /*
     * Shared value used by both DarknessPower
     * and DarkMasteryPower.
     *
     * Relics and Powers can modify this one value.
     */
    protected virtual int DarknessAmount =>
        2;

    protected int DarkMasteryThreshold =>
        30;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(
            "AttackSpGain",
            AttackSpGain),

        new DynamicVar(
            "DarkSleightSpGain",
            DarkSleightSpGain),

        new DynamicVar(
            "DarknessSpGain",
            DarknessSpGain),

        new DynamicVar(
            "DarknessRequired",
            DarknessRequired),

        new DynamicVar(
            "DarknessAmount",
            DarknessAmount),

        new DynamicVar(
            "DarkMasteryThreshold",
            DarkMasteryThreshold),

        new DynamicVar(
            "MaxSp",
            MaxSituationPoints)
    ];

    private int SituationPointsInternal
    {
        get =>
            _situationPoints;

        set
        {
            AssertMutable();

            _situationPoints =
                Math.Clamp(
                    value,
                    0,
                    MaxSituationPoints);

            UpdateDisplay();
        }
    }

    private bool IsActivatingDarkness
    {
        get =>
            _isActivatingDarkness;

        set
        {
            AssertMutable();

            _isActivatingDarkness =
                value;

            UpdateDisplay();
        }
    }

    private int DarknessProgressInternal
    {
        get =>
            _darknessProgress;

        set
        {
            AssertMutable();

            _darknessProgress =
                Math.Clamp(
                    value,
                    0,
                    GetDarknessRequired());

            UpdateDisplay();
        }
    }

    private bool DarknessPrimedInternal
    {
        get =>
            _darknessPrimed;

        set
        {
            AssertMutable();

            _darknessPrimed =
                value;

            UpdateDisplay();
        }
    }

    /*
     * This is the final shared Darkness amount.
     *
     * Add bonuses from relics and powers here so
     * DarknessPower and DarkMasteryPower always
     * use exactly the same value.
     */
    public int GetDarknessAmount()
    {
        int amount =
            DynamicVars["DarknessAmount"].IntValue;

        /*
         * Example:
         *
         * amount += Owner.Creature
         *     .GetPowerAmount<DarknessBoostPower>();
         *
         * if (Owner.GetRelic<SomeDarknessRelic>() != null)
         *     amount += 1;
         */

        return Math.Max(
            0,
            amount);
    }

    public int GetDarknessRequired()
    {
        return Math.Max(
            1,
            DynamicVars["DarknessRequired"].IntValue);
    }

    public override Task BeforeCombatStart()
    {
        SituationPointsInternal = 0;

        IsActivatingDarkness = false;
        DarknessPrimedInternal = false;
        DarknessProgressInternal = 0;

        Status =
            RelicStatus.Normal;

        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(
        CombatRoom _)
    {
        SituationPointsInternal = 0;

        IsActivatingDarkness = false;
        DarknessPrimedInternal = false;
        DarknessProgressInternal = 0;

        Status =
            RelicStatus.Normal;

        return Task.CompletedTask;
    }
    
    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return;

        GainSituationPoints(SpGainPerTurn);
        
        if (Owner.Creature.HasPower<SituationBoostPower>())
            GainSituationPoints(Owner.Creature.GetPowerAmount<SituationBoostPower>());

        await CheckDarkMastery(
            new ThrowingPlayerChoiceContext(),
            null);
    }
    
    public override async Task AfterPotionUsed(PotionModel potion, Creature? target)
    {
        if (potion is not PaopuFruit)
            return;

        if (potion.Owner != base.Owner)
            return;
        
        await CheckDarkMastery(
            new ThrowingPlayerChoiceContext(),
            null);
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        CardModel card =
            cardPlay.Card;

        if (card.Owner != Owner)
            return;

        if (!CombatManager.Instance.IsInProgress)
            return;

        /*
         * The card has already resolved by the time
         * AfterCardPlayed runs.
         *
         * Therefore, if DarknessPower was active,
         * this card already received the Darkness
         * bonus before the relic consumes it.
         */
        bool consumedDarkness =
            await TryConsumeDarkness(
                choiceContext,
                card);

        /*
         * The card that consumes Darkness must not
         * also begin building the next Darkness cycle.
         */
        if (!consumedDarkness &&
            IsValidDarknessBuilder(card))
        {
            await GainDarknessProgress(
                choiceContext);
        }

        int situationPointGain =
            GetSituationPointGain(card);

        GainSituationPoints(
            situationPointGain);

        /*
         * Darkness activation grants its own SP.
         *
         * This SP is awarded when the stored Darkness
         * is consumed by an Attack or Skill.
         */
        if (consumedDarkness)
        {
            GainSituationPoints(
                DarknessSpGain);
        }
        
        if (Owner.Creature.HasPower<DarkMasteryPower>())
        {
            GainSituationPoints(
                DarknessSpGain);
        }

        await CheckDarkMastery(
            choiceContext,
            card);
    }

    private bool IsValidDarknessBuilder(
        CardModel card)
    {
        return card.Owner == Owner &&
               CombatManager.Instance.IsInProgress &&
               card.Type == CardType.Attack &&
               card is not ISituationCard;
    }

    private bool IsValidDarknessConsumer(
        CardModel card)
    {
        if (card.Owner != Owner)
            return false;

        if (!CombatManager.Instance.IsInProgress)
            return false;

        if (card is ISituationCard)
            return false;

        if (card is WanderingShot)
            return false;

        if (card is SleightOfHand)
            return false;
        
        return card.Type == CardType.Attack ||
               card.Type == CardType.Skill;
    }

    private async Task<bool> TryConsumeDarkness(
        PlayerChoiceContext choiceContext,
        CardModel card)
    {
        if (!DarknessPrimed)
            return false;

        if (!IsValidDarknessConsumer(card))
            return false;

        await ConsumeDarkness(
            choiceContext);

        return true;
    }

    public async Task GainDarknessProgress(
        PlayerChoiceContext? choiceContext,
        int amount = 1)
    {
        if (Owner.Creature.HasPower<DarkMasteryPower>())
            return;
        
        if (amount <= 0)
            return;

        if (DarknessPrimed)
            return;

        int required =
            GetDarknessRequired();

        DarknessProgressInternal +=
            amount;

        if (DarknessProgress < required)
            return;

        DarknessProgressInternal =
            required;

        DarknessPrimedInternal =
            true;

        /*
         * DarknessPower is applied after the third
         * Attack resolves.
         *
         * The next valid Attack or Skill can therefore
         * detect DarknessPower while resolving.
         */
        if (!Owner.Creature.HasPower<DarknessPower>())
        {
            var darknessAmount = GetDarknessAmount();
            darknessAmount += Owner.Creature.GetPowerAmount<PowerOfDarknessPower>();
            
            await PowerCmd.Apply<DarknessPower>(
                choiceContext,
                Owner.Creature,
                darknessAmount,
                Owner.Creature,
                null);
        }

        TaskHelper.RunSafely(
            DoDarknessReadyVisuals());
    }
    
    public void ClearDarkness()
    {
        DarknessPrimedInternal = false;
        DarknessProgressInternal = 0;
    }

    public async Task ConsumeDarkness(
        PlayerChoiceContext? choiceContext)
    {
        DarknessPrimedInternal =
            false;

        DarknessProgressInternal =
            0;

        if (Owner.Creature.HasPower<DarknessPower>())
        {
            await PowerCmd.Remove<DarknessPower>(
                Owner.Creature);
        }
    }

    private int GetSituationPointGain(
        CardModel card)
    {
        if (card is ISituationCard)
            return 0;

        /*
         * Dark Sleights receive the Dark Sleight
         * amount instead of Attack SP.
         */
        if (card is IDarkSleight)
        {
            return DynamicVars[
                "DarkSleightSpGain"
            ].IntValue;
        }

        if (card.Type == CardType.Attack)
        {
            return DynamicVars["AttackSpGain"].IntValue + Owner.Creature.GetPowerAmount<SituationBoostPower>();

        }

        return 0;
    }

    public void GainSituationPoints(
        int amount)
    {
        if (amount <= 0)
            return;

        SituationPointsInternal +=
            amount;
    }

    public void ConsumeSituationPoints(
        int amount)
    {
        if (amount <= 0)
            return;

        SituationPointsInternal -=
            amount;
    }

    public void ConsumeAllSituationPoints()
    {
        SituationPointsInternal = 0;
    }

    public void SetSituationPoints(
        int amount)
    {
        SituationPointsInternal =
            amount;
    }

    private async Task CheckDarkMastery(
        PlayerChoiceContext? choiceContext,
        CardModel? card)
    {
        Creature creature =
            Owner.Creature;

        if (creature.HasPower<DarkMasteryPower>())
            return;

        if (SituationPoints <
            DarkMasteryThreshold)
        {
            return;
        }

        SituationPointsInternal =
            0;
        
        CommandReadyInternal = true;

        await PowerCmd.Apply<DarkMasteryPower>(
            choiceContext,
            creature,
            DarkMasteryTurns,
            creature,
            card);
    }

    private async Task DoDarknessReadyVisuals()
    {
        IsActivatingDarkness =
            true;

        Flash();

        await Cmd.Wait(1f);

        IsActivatingDarkness =
            false;
    }

    public int GetSituationPointsForUI()
    {
        return SituationPoints;
    }

    public int GetMaxSituationPointsForUI()
    {
        return MaxSituationPoints;
    }

    public int GetArrowProgressForUI()
    {
        return Math.Clamp(
            SituationPoints,
            0,
            MaxSituationPoints);
    }

    public int GetDarknessProgressForUI()
    {
        if (IsActivatingDarkness ||
            DarknessPrimed)
        {
            return GetDarknessRequired();
        }

        return DarknessProgress;
    }

    public bool IsDarknessPrimedForUI()
    {
        return DarknessPrimed;
    }

    public bool HasSituationPoints(
        int amount)
    {
        return SituationPoints >= amount;
    }

    public void SpendSituationPoints(
        int amount)
    {
        ConsumeSituationPoints(
            amount);
    }

    public int SpendAllSituationPoints()
    {
        int spent =
            SituationPoints;

        ConsumeAllSituationPoints();

        return spent;
    }

    private void UpdateDisplay()
    {
        bool active =
            IsActivatingDarkness ||
            DarknessPrimed ||
            SituationPoints >=
            DarkMasteryThreshold;

        Status =
            active
                ? RelicStatus.Active
                : RelicStatus.Normal;

        InvokeDisplayAmountChanged();
    }
}
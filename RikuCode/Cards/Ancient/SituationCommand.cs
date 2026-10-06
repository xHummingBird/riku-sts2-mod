using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Riku.RikuCode.Mechanics;
using Riku.RikuCode.Powers;
using Riku.RikuCode.Relics;

namespace Riku.RikuCode.Cards.Ancient;

public class SituationCommand() : RikuCard(
    0,
    CardType.Skill,
    CardRarity.Ancient,
    TargetType.AnyEnemy),
    ISituationCard
{
    private const int DarkCommandCost = 30;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];

    private IEnumerable<CardModel> GetSituationCommandCards()
    {
        CardPile hand =
            PileType.Hand.GetPile(Owner);

        return hand.Cards
            .OfType<SituationCommand>();
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        RikuSituationRelicBase? relic =
            DarknessHelper.GetRelic(Owner);

        if (relic == null)
        {
            return;
        }

        int situationPoints =
            relic.SituationPoints;

        bool hasDarkMastery =
            Owner.Creature.HasPower<DarkMasteryPower>();

        if (situationPoints < DarkCommandCost &&
            !hasDarkMastery)
        {
            return;
        }

        List<CardModel> availableCommands =
            CreateAvailableCommands(
                situationPoints,
                hasDarkMastery);

        /*
         * An upgraded Situation Command upgrades
         * every command offered by the selection.
         */
        if (IsUpgraded)
        {
            foreach (CardModel command in availableCommands)
            {
                if (!command.IsUpgraded)
                {
                    CardCmd.Upgrade(command);
                }
            }
        }

        CardModel selectedCommand =
            await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                availableCommands,
                Owner,
                canSkip: false);

        /*
         * Exhaust all duplicate Situation Command
         * cards still present in the Hand.
         */
        foreach (CardModel card in
                 GetSituationCommandCards().ToList())
        {
            await CardCmd.Exhaust(
                choiceContext,
                card);
        }

        /*
         * Situation Command always consumes
         * all remaining SP.
         */
        relic.ConsumeAllSituationPoints();
        
        relic.CommandReady = false;
        SfxCmd.Play("res://Riku/sfx/formchange.wav");
        await CardCmd.AutoPlay(
            choiceContext,
            selectedCommand,
            play.Target);

        if (Owner.Creature.HasPower<DarkMasteryPower>())
        {
            await PowerCmd.Remove<DarkMasteryPower>(
                Owner.Creature);
            var ownerCreature = Owner?.Creature;
            if (ownerCreature != null && Owner?.Character is Character.Riku riku) 
                riku.PlayAnimation(ownerCreature, "idle");
        }
    }

    private List<CardModel> CreateAvailableCommands(
        int situationPoints,
        bool hasDarkMastery)
    {
        /*
         * 30+ SP:
         * Dark Aura or Dark Firaga.
         */
        if (situationPoints >= DarkCommandCost)
        {
            return
            [
                CombatState.CreateCard<DarkAura>(
                    Owner),

                CombatState.CreateCard<DarkFiraga>(
                    Owner)
            ];
        }

        /*
         * 0-29 SP with Dark Mastery:
         * Ars Solum or Dark Maelstrom.
         */
        if (hasDarkMastery)
        {
            return
            [
                CombatState.CreateCard<ArsSolum>(
                    Owner),

                CombatState.CreateCard<DarkMaelstrom>(
                    Owner)
            ];
        }

        return [];
    }
}
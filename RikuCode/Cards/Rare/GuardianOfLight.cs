using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Riku.RikuCode.Relics;

namespace Riku.RikuCode.Cards.Rare;

public class GuardianOfLight() : RikuCard(1, CardType.Skill,
    CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(2),
        new DynamicVar("Sp", 15)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature, DynamicVars.Energy.BaseValue, Owner.Creature, this);

        RikuSituationRelicBase? relic = Owner.GetRelic<RikuSituationRelicBase>();

        if (relic != null)
        {
            relic.GainSituationPoints(DynamicVars["Sp"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Sp"].UpgradeValueBy(5);
    }
}
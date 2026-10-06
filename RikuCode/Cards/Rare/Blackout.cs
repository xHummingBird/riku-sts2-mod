using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Rare;

public class Blackout() : RikuCard(
    0,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<WeakPower>(1),
        new PowerVar<VulnerablePower>(1),
        new PowerVar<AmplifyWeakPower>(1),
        new PowerVar<AmplifyVulnerablePower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            CombatState.HittableEnemies,
            DynamicVars.Weak.BaseValue,
            Owner.Creature,
            this);

        await PowerCmd.Apply<VulnerablePower>(
            choiceContext,
            CombatState.HittableEnemies,
            DynamicVars.Vulnerable.BaseValue,
            Owner.Creature,
            this);

        await PowerCmd.Apply<AmplifyWeakPower>(
            choiceContext,
            CombatState.HittableEnemies,
            DynamicVars["AmplifyWeakPower"].BaseValue,
            Owner.Creature,
            this);

        await PowerCmd.Apply<AmplifyVulnerablePower>(
            choiceContext,
            CombatState.HittableEnemies,
            DynamicVars["AmplifyVulnerablePower"].BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Vulnerable.UpgradeValueBy(1);
        DynamicVars.Weak.UpgradeValueBy(1);
    }
}
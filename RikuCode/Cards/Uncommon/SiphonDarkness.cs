using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Mechanics;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Uncommon;

public class SiphonDarkness() : RikuCard(1, CardType.Power,
    CardRarity.Uncommon, TargetType.Self)
{
    protected override bool ShouldGlowGoldInternal =>
        Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(2m),
        new DynamicVar("BonusStrength", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DarknessPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars.Strength.BaseValue,
            Owner.Creature, this);

        if (Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>())
        {
            await PowerCmd.Remove<DarknessPower>(Owner.Creature);
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars["BonusStrength"].BaseValue,
                Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
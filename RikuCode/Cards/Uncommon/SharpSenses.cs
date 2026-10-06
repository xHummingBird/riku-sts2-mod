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

public class SharpSenses() : RikuCard(1, CardType.Power,
    CardRarity.Uncommon, TargetType.Self)
{
    protected override bool ShouldGlowGoldInternal =>
        Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<DexterityPower>(2m),
        new DynamicVar("BonusDexterity", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromPower<DarknessPower>(),
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, DynamicVars.Dexterity.BaseValue,
            Owner.Creature, this);

        if (Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>())
        {
            await PowerCmd.Remove<DarknessPower>(Owner.Creature);
            await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, DynamicVars["BonusDexterity"].BaseValue,
                Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
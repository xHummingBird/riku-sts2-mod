using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Riku.RikuCode.Cards.Rare;

public class DarkHarmony() : RikuCard(
    1,
    CardType.Skill,
    CardRarity.Rare,
    TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int weakAmount =
            play.Target.GetPower<WeakPower>()?.Amount ?? 0;

        int vulnerableAmount =
            play.Target.GetPower<VulnerablePower>()?.Amount ?? 0;

        int strengthAmount =
            Math.Max(
                weakAmount,
                vulnerableAmount);

        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Owner.Creature,
            strengthAmount,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
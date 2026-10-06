using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Riku.RikuCode.Cards.Rare;

public class Doubleflight() : RikuCard(1, CardType.Skill,
    CardRarity.Rare, TargetType.Self)
{
    protected override bool ShouldGlowGoldInternal => HasPlayedAttackThisTurn;
    
    private bool HasPlayedAttackThisTurn =>
        CombatManager.Instance.History.CardPlaysFinished.Any(
            e =>
                e.HappenedThisTurn(CombatState) &&
                e.CardPlay.Card.Owner == Owner &&
                e.CardPlay.Card.Type == CardType.Attack);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        if (HasPlayedAttackThisTurn)
        {
            await CommonActions.CardBlock(this, play);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Mechanics;

namespace Riku.RikuCode.Cards.Common;

public class ComboFinisher() : RikuCard(2, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    private bool HasPlayedDarkSleightThisTurn =>
        CombatManager.Instance.History.CardPlaysFinished.Any(
            e =>
                e.HappenedThisTurn(CombatState) &&
                e.CardPlay.Card.Owner == Owner &&
                e.CardPlay.Card is IDarkSleight);
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12, ValueProp.Move),
        new EnergyVar(1)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Riku riku)
        {
            AudioHelper.PlayRandomAttack();
            riku.PlayAnimation(ownerCreature, "attack");

            await Task.Delay((int)(0.100f * 1000f));

            riku.PlayVfxOnTarget(
                play.Target,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal"
            );

            SfxCmd.Play("res://Riku/sfx/riku_swing_down.wav");
        }

        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"))
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
    
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;

        if (cardPlay.Card is IDarkSleight)
            ReduceCost();

        return Task.CompletedTask;
    }
    
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this)
            return Task.CompletedTask;

        if (IsClone)
            return Task.CompletedTask;

        if (HasPlayedDarkSleightThisTurn)
        {
            ReduceCost();
        }

        return Task.CompletedTask;
    }
    
    private void ReduceCost()
    {
        base.EnergyCost.SetThisTurn(0);
    }
}

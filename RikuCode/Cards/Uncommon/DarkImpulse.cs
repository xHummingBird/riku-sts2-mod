using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Mechanics;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Uncommon;

public class DarkImpulse() : RikuCard(3, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy), IDarkSleight
{
    protected override bool ShouldGlowGoldInternal => base.Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>();
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(16, ValueProp.Move),
        new PowerVar<WeakPower>(1m),
        new PowerVar<AmplifyWeakPower>(1m),
        new EnergyVar(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<DarknessPower>(),
        RikuStaticHoverTips.DarkSleight,
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (ownerCreature != null && Owner?.Character is Character.Riku riku)
        {
            AudioHelper.PlayRandomFinalAttack();
            riku.DashTo(ownerCreature, play.Target, distance: 300f, durationSeconds: 0.3F, overrideAnim: "dark_impulse");
            await Task.Delay(200);
            SfxCmd.Play("res://Riku/sfx/From_prev/fire_impact (2).wav");
            await Task.Delay(100);
            riku.PlayVfxOnTarget(
                play.Target,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal"
            );
            SfxCmd.Play("res://Riku/sfx/riku_swing_heavy.wav");
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"))
                .Execute(choiceContext);
            await Task.Delay(400);
            await riku.Retreat(ownerCreature, "dark_impulse", false, 0.1F);
            riku.PlayAnimation(ownerCreature, "appear");
            await Task.Delay(100);
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"))
                .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
        if (Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>())
            await PowerCmd.Apply<AmplifyWeakPower>(choiceContext, play.Target, base.DynamicVars.Weak.BaseValue,
                base.Owner.Creature, this);
       
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6);
        DynamicVars["AmplifyWeakPower"].UpgradeValueBy(1);
        DynamicVars.Weak.UpgradeValueBy(1);
    }
    
    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this)
        {
            return Task.CompletedTask;
        }
        if (base.IsClone)
        {
            return Task.CompletedTask;
        }
        int amount = CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) => e.CardPlay.Card.Type == CardType.Attack && e.CardPlay.Player == base.Owner && e.HappenedThisTurn(base.CombatState));
        ReduceCostBy(amount);
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != base.Owner)
        {
            return Task.CompletedTask;
        }
        if (cardPlay.Card.Type != CardType.Attack)
        {
            return Task.CompletedTask;
        }
        ReduceCostBy(1);
        return Task.CompletedTask;
    }

    private void ReduceCostBy(int amount)
    {
        base.EnergyCost.AddThisTurn(-amount);
    }
}
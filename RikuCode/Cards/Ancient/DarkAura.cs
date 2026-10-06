using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Mechanics;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Ancient;

public class DarkAura() : RikuCard(0, CardType.Attack,
    CardRarity.Ancient, TargetType.AnyEnemy), ISituationCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(25m),
        new ExtraDamageVar(2m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((card, _) =>
            {
                return card.Owner.PlayerCombatState.AllCards
                    .Count(c => c is IDarkSleight);
            })
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (ownerCreature != null && Owner?.Character is Character.Riku riku)
        {
            SfxCmd.Play("res://Riku/sounds/transform.wav");
            riku.PlayAnimation(ownerCreature, "dark_aura");
            await Task.Delay((int)(0.6f * 1000f));
            await riku.DashTo(ownerCreature, play.Target, distance: 400f, durationSeconds: 0.05F, overrideAnim: "dark_aura");
            await Task.Delay((int)(0.05f * 1000f));
            await Task.Delay((int)(0.5f * 1000f));
            
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            riku.DashTo(ownerCreature, play.Target, distance: -350f, durationSeconds: 0.2F, overrideAnim: "dark_aura");
            await Task.Delay((int)(0.1f * 1000f));
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, "res://Riku/sfx/dark/hit_hard (2).wav", hitVfx: "hit_dark");
            await Task.Delay((int)(0.167f * 1000f));
            
            AudioHelper.PlayRandomAttack();
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            riku.DashTo(ownerCreature, play.Target, distance: -250f, durationSeconds: 0.2F, overrideAnim: "dark_aura");
            await Task.Delay((int)(0.1f * 1000f));
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, "res://Riku/sfx/dark/hit_hard (4).wav", hitVfx: "hit_dark");
            await Task.Delay((int)(0.166f * 1000f));
            
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            riku.DashTo(ownerCreature, play.Target, distance: -350f, durationSeconds: 0.2F, overrideAnim: "dark_aura");
            await Task.Delay((int)(0.1f * 1000f));
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, "res://Riku/sfx/dark/hit_hard (2).wav", hitVfx: "hit_dark");
            await Task.Delay((int)(0.167f * 1000f));
            
            AudioHelper.PlayRandomAttack();
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            riku.DashTo(ownerCreature, play.Target, distance: -250f, durationSeconds: 0.2F, overrideAnim: "dark_aura");
            await Task.Delay((int)(0.1f * 1000f));
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, "res://Riku/sfx/dark/hit_hard (4).wav", hitVfx: "hit_dark");
            await Task.Delay((int)(0.166f * 1000f));
            
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            riku.DashTo(ownerCreature, play.Target, distance: -350f, durationSeconds: 0.2F, overrideAnim: "dark_aura");
            await Task.Delay((int)(0.1f * 1000f));
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, "res://Riku/sfx/dark/hit_hard (2).wav", hitVfx: "hit_dark");
            await Task.Delay((int)(0.167f * 1000f));
            
            AudioHelper.PlayRandomAttack();
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            riku.DashTo(ownerCreature, play.Target, distance: -250f, durationSeconds: 0.2F, overrideAnim: "dark_aura");
            await Task.Delay((int)(0.1f * 1000f));
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, "res://Riku/sfx/dark/hit_hard (4).wav", hitVfx: "hit_dark");
            await Task.Delay((int)(0.167f * 1000f));
            SfxCmd.Play("res://Riku/sfx/From_prev/charge_1.wav");
            
            await Task.Delay((int)(0.633f * 1000f));
            
            AudioHelper.PlayRandomFinalAttack();
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            
            await Task.Delay((int)(0.1f * 1000f));
            SfxCmd.Play("res://Riku/sfx/From_prev/fire_impact (2).wav");
            
            await Task.Delay((int)(0.034f * 1000f));
            SfxCmd.Play("res://Riku/sfx/riku_swing_heavy.wav");
            await Task.Delay((int)(0.066f * 1000f));
            riku.PlayVfxOnTarget(
                play.Target,
                "res://Riku/scenes/vfx.tscn",
                "hit_dark"
            );
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"))
                .Execute(choiceContext);
            await Task.Delay(700);
            await riku.Retreat(ownerCreature, "dark_aura", false, 0.1F);
            riku.PlayAnimation(ownerCreature, "appear");
            await Task.Delay(100);
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"))
                .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(7);
        DynamicVars.ExtraDamage.UpgradeValueBy(1);
    }
}
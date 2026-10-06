using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Mechanics;

namespace Riku.RikuCode.Cards.Ancient;

public class ArsSolum() : RikuCard(0, CardType.Attack,
    CardRarity.Ancient, TargetType.AnyEnemy), ISituationCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(21m, ValueProp.Move),
        new PowerVar<VulnerablePower>(2m)
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
            riku.PlayAnimation(ownerCreature, "ars_solum");
            await riku.DashTo(ownerCreature, play.Target, distance: 350f, durationSeconds: 0.2F, overrideAnim: "ars_solum");
            
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, "res://Riku/sfx/dark/hit_1.wav", "res://Riku/sfx/riku_3_swing.wav", hitVfx: "hit_dark");
            await Task.Delay((int)(0.233f * 1000f));
            
            AudioHelper.PlayRandomAttack();
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, "res://Riku/sfx/dark/hit_1.wav", hitVfx: "hit_dark");
            await Task.Delay((int)(0.300f * 1000f));
            
            AudioHelper.PlayRandomAttack();
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, "res://Riku/sfx/dark/hit_hard (1).wav", "res://Riku/sfx/riku_swing_heavy.wav", hitVfx: "hit_dark");
            
            await Task.Delay((int)(0.234f * 1000f));
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            await Task.Delay((int)(0.066f * 1000f));
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, "res://Riku/sfx/dark/hit_hard (3).wav", hitVfx: "hit_dark");
            
            AudioHelper.PlayRandomAttack();
            await Task.Delay((int)(0.267f * 1000f));
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, "res://Riku/sfx/dark/hit_1.wav", "res://Riku/sfx/riku_swing_down.wav", hitVfx: "hit_dark");
            await Task.Delay((int)(0.200f * 1000f));
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, "res://Riku/sfx/dark/hit_1.wav", "res://Riku/sfx/riku_swing_down.wav", hitVfx: "hit_dark");
            AudioHelper.PlayRandomAttack();
            await Task.Delay((int)(0.267f * 1000f));
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, "res://Riku/sfx/dark/hit_hard (4).wav", "res://Riku/sfx/riku_swing_up.wav", hitVfx: "hit_dark");
            await Task.Delay((int)(0.266f * 1000f));
            AudioHelper.PlayRandomFinalAttack();
            await Task.Delay((int)(0.300f * 1000f));
            SfxCmd.Play("res://Riku/sfx/riku_swing_heavy.wav");
            riku.PlayVfxOnTarget(
                play.Target,
                "res://Riku/scenes/vfx.tscn",
                "hit_dark"
            );
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Riku/sfx/dark/hit_hard (1).wav")
                .Execute(choiceContext);
            await Task.Delay(267);
            await riku.Retreat(ownerCreature);
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, "res://Riku/sfx/dark/hit_hard (1).wav")
                .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target, base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7);
    }
}
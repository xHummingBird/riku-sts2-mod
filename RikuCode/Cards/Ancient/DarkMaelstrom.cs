using BaseLib.Extensions;
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

public class DarkMaelstrom() : RikuCard(0, CardType.Attack,
    CardRarity.Ancient, TargetType.AllEnemies), ISituationCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(16, ValueProp.Move),
        new PowerVar<WeakPower>(2m)
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
        var riku = Owner?.Character as Character.Riku;

        var enemies = CombatState.HittableEnemies.ToList();

        if (ownerCreature == null || riku == null || enemies.Count == 0)
            return;

        var rightMostEnemy = enemies[enemies.Count - 1];
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        AudioHelper.PlayRandomFinalAttack();
        // Start the Sonic attack animation.
        riku.PlayAnimation(ownerCreature, "dark_maelstrom");
        // Start the dash without awaiting it.
        await riku.DashTo(ownerCreature, rightMostEnemy, distance: 500f, durationSeconds: 0.467f, overrideAnim: "dark_maelstrom");
        foreach (var enemy in enemies)
        {
            riku.PlayVfxOnTarget(
                enemy,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature)
                    ? "hit_dark"
                    : "hit_normal"
            );
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, enemy, playHitVfx: false);
        }
        SfxCmd.Play("res://Riku/sfx/riku_swing_down.wav");
        SfxCmd.Play("res://Riku/sfx/dark/hit_hard (1).wav");
        await Task.Delay(66);
        SfxCmd.Play("res://Riku/sfx/riku_swing_up.wav");
        await Task.Delay(34);
        SfxCmd.Play("res://Riku/sfx/dark/hit_hard (2).wav");
        foreach (var enemy in enemies)
        {
            riku.PlayVfxOnTarget(
                enemy,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature)
                    ? "hit_dark"
                    : "hit_normal"
            );
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, enemy, playHitVfx: false);
        }
        await Task.Delay(167);
        SfxCmd.Play("res://Riku/sfx/riku_swing_heavy.wav");
        await Task.Delay(33);
        foreach (var enemy in enemies)
        {
            riku.PlayVfxOnTarget(
                enemy,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature)
                    ? "hit_dark"
                    : "hit_normal"
            );
        }
        await CommonActions.CardAttack(this, play)
            .WithHitFx(
                null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (4).wav")
            )
            .Execute(choiceContext);
        
        await riku.Retreat(ownerCreature, goIdle:false);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        await PowerCmd.Apply<WeakPower>(choiceContext, CombatState.HittableEnemies, base.DynamicVars.Weak.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}
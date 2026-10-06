using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Mechanics;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Rare;

public class DarkSplicer() : RikuCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies), IDarkSleight
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(25, ValueProp.Move),
        new PowerVar<WeakPower>(2),
        new DynamicVar("AmplifyWeak", 1),
        new EnergyVar(1)
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

        if (ownerCreature == null || enemies.Count == 0)
            return;
        var leftMostEnemy = enemies[0];
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        // Start the Sonic attack animation.
        riku.PlayAnimation(ownerCreature, "disappear");
        await Task.Delay(200);
        // Start the dash without awaiting it.
        riku.DashTo(ownerCreature, leftMostEnemy, distance: 250f, durationSeconds: 0.15f, overrideAnim: "disappear");
        foreach (var enemy in enemies)
        {
            await riku.DashTo(ownerCreature, enemy, distance: 250f, durationSeconds: 0.05f, overrideAnim: null);
            riku.PlayAnimation(ownerCreature, "dark_splicer");
            AudioHelper.PlayRandomAttack();
            SfxCmd.Play("res://Riku/sfx/formchange.wav");
            await Task.Delay(200);
            SfxCmd.Play("res://Riku/sfx/riku_swing_down.wav");
            SfxCmd.Play(Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"));
            riku.PlayVfxOnTarget(
                enemy,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal"
            );
            await Task.Delay(167);
            SfxCmd.Play("res://Riku/sfx/riku_swing_down.wav");
            SfxCmd.Play(Character.Riku.GetSfxPath(ownerCreature, "hit_soft.wav"));
            riku.PlayVfxOnTarget(
                enemy,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal"
            );
            await Task.Delay(266);
            SfxCmd.Play("res://Riku/sfx/riku_swing_up.wav");
            SfxCmd.Play(Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"));
            riku.PlayVfxOnTarget(
                enemy,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal"
            );
            await Task.Delay(167);
            SfxCmd.Play("res://Riku/sfx/riku_swing_heavy.wav");
            SfxCmd.Play(Character.Riku.GetSfxPath(ownerCreature, "hit_hard (2).wav"));
            riku.PlayVfxOnTarget(
                enemy,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal"
            );
            await Task.Delay(350);
        }
        await riku.Retreat(ownerCreature, animation: null, false, 0.1F);
        riku.PlayAnimation(ownerCreature, "appear");
        await Task.Delay(300);
        foreach (var enemy in enemies)
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, enemy,
                Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"),
                hitVfx: Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
        await Task.Delay(150);
        foreach (var enemy in enemies)
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, enemy,
                Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"),
                hitVfx: Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
        await Task.Delay(150);
        foreach (var enemy in enemies)
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, enemy,
                Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"),
                hitVfx: Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
        await Task.Delay(150);
        foreach (var enemy in enemies)
            riku.PlayVfxOnTarget(
                enemy,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal"
            );
        await CommonActions.CardAttack(this, play)
                .WithHitFx(
                    null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav")
                )
                .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        await PowerCmd.Apply<WeakPower>(choiceContext, CombatState.HittableEnemies, base.DynamicVars.Weak.BaseValue, Owner.Creature, this);
        if (Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>())
        {
            await PowerCmd.Apply<AmplifyWeakPower>(choiceContext, CombatState.HittableEnemies,
                base.DynamicVars["AmplifyWeak"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage. UpgradeValueBy(5);
        DynamicVars["AmplifyWeak"].UpgradeValueBy(1);
    }
    
    // public override Task AfterCardEnteredCombat(CardModel card)
    // {
    //     if (card != this)
    //         return Task.CompletedTask;
    //
    //     if (IsClone)
    //         return Task.CompletedTask;
    //
    //     int count = CombatManager.Instance.History.CardPlaysFinished.Count(
    //         e =>
    //             e.CardPlay.Player == Owner &&
    //             e.CardPlay.Card is IDarkSleight);
    //
    //     EnergyCost.AddThisCombat(-count * DynamicVars.Energy.IntValue);
    //
    //     return Task.CompletedTask;
    // }
    //
    // public override Task AfterCardPlayed(
    //     PlayerChoiceContext choiceContext,
    //     CardPlay cardPlay)
    // {
    //     if (cardPlay.Card.Owner != Owner)
    //         return Task.CompletedTask;
    //
    //     if (cardPlay.Card is not IDarkSleight)
    //         return Task.CompletedTask;
    //
    //     EnergyCost.AddThisCombat(-DynamicVars.Energy.IntValue);
    //
    //     return Task.CompletedTask;
    // }
}
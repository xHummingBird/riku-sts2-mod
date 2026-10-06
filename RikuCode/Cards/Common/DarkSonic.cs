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

namespace Riku.RikuCode.Cards.Common;

public class DarkSonic() : RikuCard(
    1,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AllEnemies), IDarkSleight
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move),
        new PowerVar<VulnerablePower>(1),
        new DynamicVar("AmplifyVulnerable", 1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<DarknessPower>(),
        RikuStaticHoverTips.DarkSleight,
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

        var rightMostEnemy = enemies[enemies.Count - 1];

        AudioHelper.PlayRandomFinalAttack();
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        // Start the Sonic attack animation.
        riku.PlayAnimation(ownerCreature, "sonic");

        await Task.Delay(100);
        SfxCmd.Play("res://Riku/sfx/riku_swing_heavy.wav");
        // Start the dash without awaiting it.
        riku.DashTo(ownerCreature, rightMostEnemy, distance: -250f, durationSeconds: 0.1f, overrideAnim: "sonic");
        
        await Task.Delay(50);
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
                    null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav")
                )
                .Execute(choiceContext);
        
        await riku.Retreat(ownerCreature);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, CombatState.HittableEnemies, base.DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
        if (Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>())
            await PowerCmd.Apply<AmplifyVulnerablePower>(choiceContext, CombatState.HittableEnemies, base.DynamicVars["AmplifyVulnerable"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars.Vulnerable.UpgradeValueBy(1);
    }
}

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Mechanics;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Rare;

public class Zantetsuken() : RikuCard(
    2,
    CardType.Attack,
    CardRarity.Rare,
    TargetType.AnyEnemy), IDarkSleight
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(20, ValueProp.Move),
        new DynamicVar("hpPercent", 8)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        RikuStaticHoverTips.DarkSleight,
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;
        var riku = Owner?.Character as Character.Riku;
        decimal threshold = DynamicVars["hpPercent"].BaseValue;

        SfxCmd.Play("res://Riku/sounds/transform.wav");
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        // Start the Sonic attack animation.
        riku.PlayAnimation(ownerCreature, "zantetsuken");

        await Task.Delay(250);
        SfxCmd.Play("res://Riku/sfx/riku_swing_heavy.wav");
        await riku.DashTo(ownerCreature, play.Target, distance: -350f, durationSeconds: 0.05f, overrideAnim: "zantetsuken");
        riku.PlayVfxOnTarget(
            play.Target,
            "res://Riku/scenes/vfx.tscn",
            Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal"
            );
        await CommonActions.CardAttack(this, play)
                .WithHitFx(
                    null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav")
                )
                .Execute(choiceContext);
        await Task.Delay(100);
        await riku.Retreat(ownerCreature);
        if (Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>())
        {
            if (play.Target.CurrentHp * 100 <= play.Target.MaxHp * threshold && play.Target.CurrentHp > 0)
            {
                await DoomPower.DoomKill(new List<Creature> { play.Target });
                return;
            }
        }
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7);
        DynamicVars["hpPercent"].UpgradeValueBy(3m);
    }
}
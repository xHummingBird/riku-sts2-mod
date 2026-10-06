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

namespace Riku.RikuCode.Cards.Basic;

public class ShadowStrike() : RikuCard(0, CardType.Attack,
    CardRarity.Basic, TargetType.AnyEnemy), IDarkSleight
{
    protected override bool ShouldGlowGoldInternal => base.Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>();
    
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(3, ValueProp.Move),
        new PowerVar<WeakPower>(1m)
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
        var hitCount = 1;
        if (ownerCreature.HasPower<DarknessPower>() || ownerCreature.HasPower<DarkMasteryPower>())
            hitCount = 2;
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);
        if (ownerCreature != null && Owner?.Character is Character.Riku riku)
        {
            AudioHelper.PlayRandomFinalAttack();
            await riku.DashTo(ownerCreature, play.Target, distance: 300f, durationSeconds: 0.3F, overrideAnim: "break");
            
            riku.PlayVfxOnTarget(
                play.Target,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal"
            );
            SfxCmd.Play("res://Riku/sfx/riku_swing_heavy.wav");
            await CommonActions.CardAttack(this, play.Target, hitCount: hitCount)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"))
                .Execute(choiceContext);
            await Task.Delay(400);
            await riku.Retreat(ownerCreature, "break", false, 0.1F);
            riku.PlayAnimation(ownerCreature, "appear");
            await Task.Delay(100);
        }
        else
            await CommonActions.CardAttack(this, play.Target, hitCount: hitCount)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"))
                .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        await PowerCmd.Apply<WeakPower>(choiceContext, play.Target, base.DynamicVars.Weak.BaseValue,
            base.Owner.Creature, this);
       
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
        DynamicVars.Weak.UpgradeValueBy(1);
    }
}
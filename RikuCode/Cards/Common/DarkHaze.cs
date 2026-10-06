using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
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

namespace Riku.RikuCode.Cards.Common;

public class DarkHaze() : RikuCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy), IDarkSleight
{
    protected override bool ShouldGlowGoldInternal => (base.Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>());
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(10, ValueProp.Move),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DarknessPower>(),
        RikuStaticHoverTips.DarkSleight,
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        int enemyWeak = play.Target.GetPowerAmount<WeakPower>();
        var ownerCreature = Owner?.Creature;
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
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"))
                .Execute(choiceContext);
            await Task.Delay(400);
            await riku.Retreat(ownerCreature, "break", false, 0.1F);
            riku.PlayAnimation(ownerCreature, "appear");
            await Task.Delay(100);
        }
        else
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"))
                .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
        if (ownerCreature.HasPower<DarknessPower>() || ownerCreature.HasPower<DarkMasteryPower>())
        {
            await CardPileCmd.Draw(
                choiceContext,
                enemyWeak,
                Owner);
        }
        PowerCmd.Apply<WeakPower>(choiceContext, play.Target, enemyWeak, Owner.Creature, this);
        
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}
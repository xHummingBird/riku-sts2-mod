using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Mechanics;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Uncommon;

public class DarkBarrage() : RikuCard(0, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy), IDarkSleight
{
    protected override bool ShouldGlowGoldInternal => base.Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>();
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0m),
        new ExtraDamageVar(4m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((card, _) =>
            {
                int attacksPlayed =
                    CombatManager.Instance.History.CardPlaysFinished.Count(e =>
                        e.HappenedThisTurn(card.CombatState) &&
                        e.CardPlay.Card.Type == CardType.Attack &&
                        e.CardPlay.Card.Owner == card.Owner);

                bool doubled =
                    card.Owner.HasPower<DarkMasteryPower>() ||
                    card.Owner.HasPower<DarknessPower>();

                return doubled
                    ? attacksPlayed * 2
                    : attacksPlayed;
            })
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
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
            riku.PlayAnimation(ownerCreature, "cast");
            riku.PlayVfxOnTarget(
                play.Target,
                "res://Riku/scenes/vfx.tscn",
                "dark_barrage"
            );
            await Task.Delay(200);
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            await Task.Delay(100);
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"), "res://Riku/sfx/riku_thrust.wav", hitVfx: Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            await Task.Delay(100);
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (2).wav"), hitVfx: Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            await Task.Delay(100);
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            await Task.Delay(100);
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (3).wav"), hitVfx: Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            await Task.Delay(100);
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            await Task.Delay(100);
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (4).wav"), hitVfx: Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            await Task.Delay(100);
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            await Task.Delay(100);
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"), hitVfx: Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            await Task.Delay(100);
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            await Task.Delay(100);
            RikuExtensions.CombatHelpers.RikuFakeHit(ownerCreature, play.Target, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (2).wav"), hitVfx: Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
            await Task.Delay(100);
            SfxCmd.Play("res://Riku/sfx/riku_thrust.wav");
            await Task.Delay(100);
            riku.PlayVfxOnTarget(play.Target, "res://Riku/scenes/vfx.tscn", Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal");
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (3).wav"))
            .Execute(choiceContext);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(2);
    }
}
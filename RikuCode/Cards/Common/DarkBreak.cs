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
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Mechanics;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Cards.Common;

public class DarkBreak() : RikuCard(0, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy), IDarkSleight
{
    protected override bool ShouldGlowGoldInternal => (base.Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>()) || PlayCountThisTurn == 0;
    
    private int PlayCountThisTurn =>
        CombatManager.Instance.History.CardPlaysFinished
            .Count(e =>
                e.HappenedThisTurn(base.CombatState) &&
                e.CardPlay.Player == base.Owner);
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new DamageVar(4, ValueProp.Move),
        new CardsVar(1)
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
        if (PlayCountThisTurn == 0)
        {
            await CardPileCmd.Draw(
                choiceContext,
                base.DynamicVars.Cards.BaseValue,
                base.Owner);
        }

        if (ownerCreature.HasPower<DarknessPower>() || ownerCreature.HasPower<DarkMasteryPower>())
        {
            if (ownerCreature.HasPower<DarknessPower>())
            {
                var relic = DarknessHelper.GetRelic(Owner);
                if (relic != null)
                {
                    await relic.ConsumeDarkness(choiceContext);
                }
            }

            CardModel? attack = PileType.Draw.GetPile(Owner).Cards
                .Where(c =>
                    c.Type == CardType.Attack &&
                    !c.Keywords.Contains(CardKeyword.Unplayable))
                .ToList()
                .StableShuffle(Owner.RunState.Rng.Shuffle)
                .FirstOrDefault();

            if (attack != null)
            {
                if (IsUpgraded)
                {
                    CardCmd.Upgrade(attack);
                }
                await CardCmd.AutoPlay(
                    choiceContext,
                    attack,
                    null
                );
            }
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}
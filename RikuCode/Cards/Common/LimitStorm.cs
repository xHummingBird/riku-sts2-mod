using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Relics;

namespace Riku.RikuCode.Cards.Common;

public class LimitStorm() : RikuCard(
    2,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AllEnemies)
{
    protected override bool ShouldGlowGoldInternal => (Owner.Creature.CurrentHp * 100m / Owner.Creature.MaxHp) <= 30m;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(11m),
        new ExtraDamageVar(11m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((CardModel card, Creature? _) =>
            {
                var owner = card.Owner?.Creature;

                decimal hpPct =
                    owner.CurrentHp * 100m / owner.MaxHp;

                return hpPct <= 30m ? 1 : 0;
            }),
        new DynamicVar("Sp", 3)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Riku riku)
        {
            AudioHelper.PlayRandomAttack();
            riku.PlayAnimation(ownerCreature, "attack");

            await Task.Delay(100);

            var enemies = CombatState.HittableEnemies.ToList();

            foreach (var enemy in enemies)
            {
                riku.PlayVfxOnTarget(
                    enemy,
                    "res://Riku/scenes/vfx.tscn",
                    Character.Riku.IsInDarkness(ownerCreature)
                        ? "hit_dark"
                        : "hit_normal");
            }

            SfxCmd.Play("res://Riku/sfx/riku_swing_down.wav");
        }

        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"))
            .Execute(choiceContext);

        int spGain = DynamicVars["Sp"].IntValue;

        if (ownerCreature != null)
        {
            decimal hpPct = ownerCreature.CurrentHp * 100m / ownerCreature.MaxHp;

            if (hpPct <= 30m)
                spGain *= 2;
        }

        RikuSituationRelicBase? relic = Owner.GetRelic<RikuSituationRelicBase>();

        if (relic != null)
            relic.GainSituationPoints(spGain);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(4);
        DynamicVars.ExtraDamage.UpgradeValueBy(4);
        DynamicVars["Sp"].UpgradeValueBy(1);
    }
}
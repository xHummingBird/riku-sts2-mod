using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Relics;

namespace Riku.RikuCode.Cards.Common;

public class BrutalStrike() : RikuCard(
    1,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AllEnemies)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, ValueProp.Move),
        new DynamicVar("Sp", 2)
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
                        : "hit_normal"
                );
            }
            SfxCmd.Play("res://Riku/sfx/riku_swing_down.wav");
        }

        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"))
            .Execute(choiceContext);

        RikuSituationRelicBase? relic = Owner.GetRelic<RikuSituationRelicBase>();

        if (relic != null)
        {
            relic.GainSituationPoints(DynamicVars["Sp"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["Sp"].UpgradeValueBy(1);
    }
}
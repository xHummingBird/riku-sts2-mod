using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;

namespace Riku.RikuCode.Cards.Common;

public class AerialSlam() : RikuCard(
    2,
    CardType.Attack,
    CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15, ValueProp.Move),
        new PowerVar<DrawCardsNextTurnPower>(2m)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CenterCardCinematic.Start(RunManager.Instance.NetService.NetId);

        var ownerCreature = Owner?.Creature;

        if (ownerCreature != null && Owner?.Character is Character.Riku riku)
        {
            AudioHelper.PlayRandomFinalAttack();

            await riku.DashTo(
                ownerCreature,
                play.Target,
                distance: 300f,
                durationSeconds: 0.3f,
                overrideAnim: "break");

            riku.PlayVfxOnTarget(
                play.Target,
                "res://Riku/scenes/vfx.tscn",
                Character.Riku.IsInDarkness(ownerCreature)
                    ? "hit_dark"
                    : "hit_normal");

            SfxCmd.Play("res://Riku/sfx/riku_swing_heavy.wav");

            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"))
                .Execute(choiceContext);

            await Task.Delay(400);

            await riku.Retreat(ownerCreature, "break", false, 0.1f);
            riku.PlayAnimation(ownerCreature, "appear");

            await Task.Delay(100);
        }
        else
        {
            await CommonActions.CardAttack(this, play.Target)
                .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_hard (1).wav"))
                .Execute(choiceContext);
        }

        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);

        await PowerCmd.Apply<DrawCardsNextTurnPower>(
            choiceContext,
            base.Owner.Creature,
            base.DynamicVars["DrawCardsNextTurnPower"].BaseValue,
            base.Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}
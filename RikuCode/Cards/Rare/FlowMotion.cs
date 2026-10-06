using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Extensions;

namespace Riku.RikuCode.Cards.Rare;

public class FlowMotion() : RikuCard(1, CardType.Attack,
    CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var ownerCreature = Owner?.Creature;

        await CommonActions.CardBlock(this, play);

        if (ownerCreature != null && Owner?.Character is Character.Riku riku)
        {
            AudioHelper.PlayRandomAttack();
            riku.PlayAnimation(ownerCreature, "attack");

            await Task.Delay((int)(0.100f * 1000f));
            var enemies = CombatState.HittableEnemies;
                riku.PlayVfxOnTarget(
                    play.Target,
                    "res://Riku/scenes/vfx.tscn",
                    Character.Riku.IsInDarkness(ownerCreature) ? "hit_dark" : "hit_normal"
                );

            SfxCmd.Play("res://Riku/sfx/riku_swing_down.wav");
        }

        await DamageCmd.Attack(DynamicVars.Block.PreviewValue)
            .FromCard(this, play)
            .WithValueProp(ValueProp.Unpowered)
            .Targeting(play.Target)
            .WithHitFx(null, Character.Riku.GetSfxPath(ownerCreature, "hit_1.wav"))
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4);
    }
}

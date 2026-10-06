using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
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

namespace Riku.RikuCode.Cards.Uncommon;

public class WanderingShot() : RikuCard(2, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy), IDarkSleight
{
    protected override bool ShouldGlowGoldInternal =>
        base.Owner.HasPower<DarknessPower>() || Owner.HasPower<DarkMasteryPower>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move),
        new EnergyVar(1)
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
                "wandering_shot"
            );
            await Task.Delay(100);
        }
        await CommonActions.CardAttack(this, play.Target)
            .WithHitFx(null, "res://Riku/sfx/From_prev/from_tifa/chi_sfx.wav")
            .Execute(choiceContext);
        await PowerCmd.Apply<FreeAttackPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        CenterCardCinematic.End(RunManager.Instance.NetService.NetId);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}
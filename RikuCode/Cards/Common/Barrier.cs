using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Riku.RikuCode.Relics;

namespace Riku.RikuCode.Cards.Common;

public class Barrier() : RikuCard(1, CardType.Skill,
    CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new BlockVar(7, ValueProp.Move),
            new DynamicVar("Sp", 2)
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        RikuSituationRelicBase? relic = Owner.GetRelic<RikuSituationRelicBase>();
            
        if (relic != null)
        {
            relic.GainSituationPoints(DynamicVars["Sp"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(3m);
        DynamicVars["Sp"].UpgradeValueBy(1);
    }
}
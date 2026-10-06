using MegaCrit.Sts2.Core.Entities.Relics;

namespace Riku.RikuCode.Relics;

public class TrueBraveheart : RikuSituationRelicBase
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    protected override int AttackSpGain => 3;

    protected override int SpGainPerTurn => 2;

    protected override int DarkSleightSpGain => 3;

    protected override int DarknessSpGain => 3;

    protected override int DarknessRequired => 3;

    protected override int DarknessAmount => 2;

    protected override int DarkMasteryTurns => 5;
}
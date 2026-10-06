using MegaCrit.Sts2.Core.Entities.Relics;

namespace Riku.RikuCode.Relics;


public class Braveheart : RikuSituationRelicBase
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    protected override int AttackSpGain => 2;

    protected override int SpGainPerTurn => 1;

    protected override int DarkSleightSpGain => 3;

    protected override int DarknessSpGain => 2;

    protected override int DarknessRequired => 3;

    protected override int DarknessAmount => 2;

    protected override int DarkMasteryTurns => 4;
}
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Riku.RikuCode.Powers;

public class PowerOfDarknessPower : RikuPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
}
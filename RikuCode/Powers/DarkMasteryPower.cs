using MegaCrit.Sts2.Core.Entities.Powers;

namespace Riku.RikuCode.Powers;

public class DarknessMasteryPower  : RikuPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool AllowNegative => false;
}
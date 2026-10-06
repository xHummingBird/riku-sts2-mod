using BaseLib.Abstracts;
using BaseLib.Extensions;
using Riku.RikuCode.Extensions;
using Godot;

namespace Riku.RikuCode.Powers;

public abstract class RikuPower : CustomPowerModel
{
    //Loads from Riku/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
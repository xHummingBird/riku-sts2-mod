using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Riku.RikuCode.Character;
using Riku.RikuCode.Extensions;
using Godot;

namespace Riku.RikuCode.Relics;

[Pool(typeof(RikuRelicPool))]
public abstract class RikuRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    protected override string PackedIconOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".BigRelicImagePath();

    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}
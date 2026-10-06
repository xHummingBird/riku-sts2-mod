using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Riku.RikuCode.Character;
using Riku.RikuCode.Extensions;

namespace Riku.RikuCode.Potions;

[Pool(typeof(RikuPotionPool))]
public abstract class RikuPotion : CustomPotionModel
{
    protected string PotionFileName =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";

    protected string PotionOutlineFileName =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png";

    public override string? CustomPackedImagePath =>
        PotionFileName.PotionImagePath();

    public override string? CustomPackedOutlinePath =>
        PotionOutlineFileName.PotionImagePath();
}
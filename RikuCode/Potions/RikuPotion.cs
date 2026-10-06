using BaseLib.Abstracts;
using BaseLib.Utils;
using Riku.RikuCode.Character;

namespace Riku.RikuCode.Potions;

[Pool(typeof(RikuPotionPool))]
public abstract class RikuPotion : CustomPotionModel;
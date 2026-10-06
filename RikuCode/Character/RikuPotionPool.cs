using BaseLib.Abstracts;
using Riku.RikuCode.Extensions;
using Godot;

namespace Riku.RikuCode.Character;

public class RikuPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Riku.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
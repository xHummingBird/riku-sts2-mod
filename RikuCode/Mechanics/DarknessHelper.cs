using MegaCrit.Sts2.Core.Entities.Players;
using Riku.RikuCode.Powers;
using Riku.RikuCode.Relics;

namespace Riku.RikuCode.Mechanics;

public static class DarknessHelper
{
    public static RikuSituationRelicBase? GetRelic(
        Player player)
    {
        return player.Relics
            .OfType<RikuSituationRelicBase>()
            .FirstOrDefault();
    }

    public static int GetDarknessProgress(
        Player player)
    {
        return GetRelic(player)?
                   .GetDarknessProgressForUI()
               ?? 0;
    }

    public static bool IsDarknessPrimed(
        Player player)
    {
        return GetRelic(player)?
                   .IsDarknessPrimedForUI()
               ?? false;
    }

    public static int GetDarknessAmount(
        Player player)
    {
        return GetRelic(player)?
                   .GetDarknessAmount()
               ?? 0;
    }

    public static bool HasDarkMastery(
        Player player)
    {
        return player.Creature
            .HasPower<DarkMasteryPower>();
    }
}
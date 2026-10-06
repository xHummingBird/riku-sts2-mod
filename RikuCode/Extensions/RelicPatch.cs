using HarmonyLib;
using Riku.RikuCode.Cards.Ancient;
using Riku.RikuCode.Cards.Basic;
using Riku.RikuCode.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Riku.RikuCode.Extensions;

[HarmonyPatch(typeof(TouchOfOrobas), "GetUpgradedStarterRelic")]
internal static class RikuTouchOfOrobasPatch
{
    private static void Postfix(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic is Braveheart)
        {
            __result = ModelDb.Relic<TrueBraveheart>().ToMutable();
        }
    }
}


[HarmonyPatch(typeof(ArchaicTooth), "TranscendenceUpgrades", MethodType.Getter)]
internal static class RikuArchaicToothTranscendencePatch
{
    [HarmonyPostfix]
    private static void Postfix(ref Dictionary<ModelId, CardModel> __result)
    {
        __result[ModelDb.Card<FullCombo>().Id] = ModelDb.Card<ComboRush>();
    }
}

[HarmonyPatch(typeof(DustyTome), nameof(DustyTome.SetupForPlayer))]
public static class DustyTomeSetupPatch
{
    [HarmonyPostfix]
    public static void Postfix(DustyTome __instance, Player player)
    {
        if (player.Character is not Character.Riku)
            return;

        __instance.AncientCard = ModelDb.Card<TrueEnd>().Id;
    }
}

[HarmonyPatch(typeof(DustyTome), nameof(DustyTome.AfterObtained))]
public static class DustyTomePatch
{
    [HarmonyPrefix]
    public static void Prefix(DustyTome __instance)
    {
        if (__instance.Owner?.Character is not Character.Riku)
            return;
        
        __instance.AncientCard = ModelDb.Card<TrueEnd>().Id;
    }
}
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Riku.RikuCode.Mechanics;

[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
public static class DarkSleightUiUpdate
{
    public static void Postfix(NCard __instance)
    {
        DarkSleightOverlay.Ensure(__instance);
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard._Ready))]
public static class DarkSleightUiReady
{
    public static void Postfix(NCard __instance)
    {
        Callable.From(() =>
        {
            DarkSleightOverlay.Ensure(__instance);
        }).CallDeferred();
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard._Ready))]
public static class DarkSleightUiModelChanged
{
    public static void Postfix(NCard __instance)
    {
        __instance.ModelChanged += _ =>
        {
            Callable.From(() =>
            {
                DarkSleightOverlay.Ensure(__instance);
            }).CallDeferred();
        };
    }
}
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Riku.RikuCode.Extensions;

public static class Optimisation
{
    private static PackedScene? _iceScene;
    private static PackedScene? _vfxScene;
    private static PackedScene? _rikuScene;
    
    // private const string SoraScenePath = "res://Riku/scenes/sora.tscn";
    private const string IceVfxPath = "res://Riku/scenes/ice_vfx.tscn";
    private const string VfxPath = "res://Riku/scenes/vfx.tscn";
    private const string RikuScenePath = "res://Riku/scenes/riku.tscn";
    // private const string KairiScenePath = "res://Riku/scenes/kairi.tscn";
    
    public static PackedScene? VfxScene
    {
        get
        {
            _vfxScene = LoadOrReload(_vfxScene, VfxPath, "VFX");
            return _vfxScene;
        }
    }
    
    public static PackedScene? RikuScene
    {
        get
        {
            _rikuScene = LoadOrReload(_rikuScene, RikuScenePath, "Riku scene");
            return _rikuScene;
        }
    }
    
    private static PackedScene? LoadOrReload(PackedScene? cachedScene, string path, string label)
    {
        if (cachedScene != null && GodotObject.IsInstanceValid(cachedScene))
            return cachedScene;

        GD.Print($"RikuAssets: Loading {label} from {path}");

        var scene = GD.Load<PackedScene>(path);

        if (scene == null)
        {
            GD.PrintErr($"RikuAssets: FAILED to load {label}: {path}");
            return null;
        }

        GD.Print($"RikuAssets: Loaded {label}");
        return scene;
    }
    
    public static void EnsurePreloaded()
    {
        _ = VfxScene;
        _ = RikuScene;

        GD.Print("RikuAssets: EnsurePreloaded finished");
    }
}

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterActEntered))]
public static class RikuAfterActEnteredPreloadPatch
{
    [HarmonyPrefix]
    public static void Prefix(IRunState runState)
    {
        var player = runState?.Players?.FirstOrDefault();

        if (player?.Character is not Character.Riku)
            return;

        GD.Print("AfterActEntered: Riku detected → preloading");

        Optimisation.EnsurePreloaded();
    }
}


[HarmonyPatch(typeof(Hook), nameof(Hook.AfterRoomEntered))]
public static class RikuAfterRoomEnteredPreloadPatch
{
    [HarmonyPrefix]
    public static void Prefix(IRunState runState, AbstractRoom room)
    {
        var player = runState?.Players?.FirstOrDefault();

        if (player?.Character is not Character.Riku)
            return;

        GD.Print($"AfterRoomEntered: Riku detected → preloading. Room = {room.GetType().Name}");

        Optimisation.EnsurePreloaded();
    }
}
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using Riku.RikuCode.Extensions;
using Riku.RikuCode.Powers;

namespace Riku.RikuCode.Mechanics;

public partial class DarknessDisplayOverlay : Control
{
    public static DarknessDisplayOverlay? Instance { get; private set; }

    private Control? _display;

    private TextureRect? _overlay;
    private TextureRect? _darkMastery;

    private TextureRect? _darkness1;
    private TextureRect? _darkness2;
    private TextureRect? _darkness3;

    private AnimationPlayer? _anim;

    private Player? _player;
    private IHoverTip? _hoverTip;

    private int _lastProgress = -1;
    private bool? _lastDarkMasteryState;

    public override void _Ready()
    {
        Instance = this;
        Name = "DarknessDisplayOverlay";

        MouseFilter =
            MouseFilterEnum.Pass;

        CallDeferred(nameof(Setup));
    }

    private async void Setup()
    {
        if (!IsInsideTree())
            return;

        for (int i = 0; i < 60; i++)
        {
            var state =
                CombatManager.Instance?.DebugOnlyGetState();

            var player =
                state?.Players.FirstOrDefault(
                    p => LocalContext.IsMe(p));

            if (player != null)
            {
                if (player.Character is not Character.Riku)
                {
                    QueueFree();
                    return;
                }

                _player = player;
                break;
            }

            var tree = GetTree();

            if (tree == null)
                return;

            await ToSignal(
                tree,
                SceneTree.SignalName.ProcessFrame);
        }

        if (_player == null)
        {
            QueueFree();
            return;
        }

        var scene =
            GD.Load<PackedScene>(
                "res://Riku/scenes/darkness_display.tscn");

        if (scene == null)
        {
            GD.PushError(
                "[Riku Darkness] Failed to load darkness_display.tscn");

            QueueFree();
            return;
        }

        _display =
            scene.Instantiate<Control>();

        AddChild(_display);

        _display.MouseFilter =
            MouseFilterEnum.Pass;

        _display.SetAnchorsPreset(
            LayoutPreset.BottomLeft);

        _display.Position =
            new Vector2(
                -90,
                -70);

        /*
         * Normal Darkness overlay.
         */
        _overlay =
            _display.GetNodeOrNull<TextureRect>(
                "Overlay");

        /*
         * Dark Mastery replacement overlay.
         */
        _darkMastery =
            _display.GetNodeOrNull<TextureRect>(
                "DarkMastery");

        _darkness1 =
            _display.GetNodeOrNull<TextureRect>(
                "darkness1");

        _darkness2 =
            _display.GetNodeOrNull<TextureRect>(
                "darkness2");

        _darkness3 =
            _display.GetNodeOrNull<TextureRect>(
                "darkness3");

        _anim =
            _display.GetNodeOrNull<AnimationPlayer>(
                "AnimationPlayer");

        if (_overlay == null ||
            _darkMastery == null ||
            _darkness1 == null ||
            _darkness2 == null ||
            _darkness3 == null)
        {
            GD.PushError(
                "[Riku Darkness] darkness_display.tscn must contain Overlay, DarkMastery, darkness1, darkness2, and darkness3.");

            QueueFree();
            return;
        }

        _anim?.Play("spin");

        _hoverTip =
            RikuStaticHoverTips.Darkness;

        _display.MouseEntered +=
            OnHovered;

        _display.MouseExited +=
            OnUnhovered;

        /*
         * Normal overlay is initially visible.
         */
        _overlay.Visible = true;
        _darkMastery.Visible = false;

        _darkness1.Visible = false;
        _darkness2.Visible = false;
        _darkness3.Visible = false;

        UpdateDisplay();
    }

    public override void _Process(
        double delta)
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_player == null)
            return;

        if (_overlay == null ||
            _darkMastery == null ||
            _darkness1 == null ||
            _darkness2 == null ||
            _darkness3 == null)
        {
            return;
        }

        /*
         * This must run before checking whether the
         * Darkness progress changed.
         *
         * Dark Mastery can be gained or removed
         * without changing Darkness progress.
         */
        UpdateOverlay();

        int progress =
            DarknessHelper.GetDarknessProgress(
                _player);

        progress =
            Mathf.Clamp(
                progress,
                0,
                3);

        if (progress == _lastProgress)
            return;

        int previousProgress =
            _lastProgress;

        /*
         * Darkness gained.
         */
        if (progress > previousProgress)
        {
            if (previousProgress < 1 &&
                progress >= 1)
            {
                PlayGainAnimation(
                    _darkness1);
            }

            if (previousProgress < 2 &&
                progress >= 2)
            {
                PlayGainAnimation(
                    _darkness2);
            }

            if (previousProgress < 3 &&
                progress >= 3)
            {
                PlayGainAnimation(
                    _darkness3);
            }
        }
        /*
         * Full Darkness activation consumed all
         * three Darkness charges.
         */
        else if (previousProgress == 3 &&
                 progress == 0)
        {
            PlayConsumeAnimation();

            _lastProgress =
                progress;

            return;
        }

        _lastProgress =
            progress;

        _darkness1.Visible =
            progress >= 1;

        _darkness2.Visible =
            progress >= 2;

        _darkness3.Visible =
            progress >= 3;
    }
    
    

    private void UpdateOverlay()
    {
        if (_player == null ||
            _overlay == null ||
            _darkMastery == null)
        {
            return;
        }

        bool hasDarkMastery =
            _player.Creature
                .HasPower<DarkMasteryPower>();

        if (_lastDarkMasteryState == hasDarkMastery)
            return;

        _lastDarkMasteryState =
            hasDarkMastery;

        _overlay.Visible =
            !hasDarkMastery;

        _darkMastery.Visible =
            hasDarkMastery;

        _hoverTip =
            hasDarkMastery
                ? RikuStaticHoverTips.DarkMastery
                : RikuStaticHoverTips.Darkness;
    }

    private static void PlayGainAnimation(
        Control darkness)
    {
        darkness.Visible = true;

        darkness.Scale =
            new Vector2(
                2f,
                2f);

        var tween =
            darkness.CreateTween();

        tween.TweenProperty(
                darkness,
                "scale",
                Vector2.One,
                0.5f)
            .SetTrans(
                Tween.TransitionType.Back)
            .SetEase(
                Tween.EaseType.Out);
    }

    private static void AnimateDarknessConsume(
        TextureRect? darkness)
    {
        if (darkness == null)
            return;

        darkness.Scale =
            Vector2.One;

        darkness.Modulate =
            Colors.White;

        var tween =
            darkness.CreateTween();

        tween.Parallel()
            .TweenProperty(
                darkness,
                "scale",
                new Vector2(
                    2f,
                    2f),
                0.5f)
            .SetTrans(
                Tween.TransitionType.Back)
            .SetEase(
                Tween.EaseType.Out);

        tween.Parallel()
            .TweenProperty(
                darkness,
                "modulate:a",
                0f,
                0.5f);

        tween.TweenCallback(
            Callable.From(() =>
            {
                darkness.Visible = false;
                darkness.Scale = Vector2.One;
                darkness.Modulate = Colors.White;
            }));
    }

    private void PlayConsumeAnimation()
    {
        AnimateDarknessConsume(
            _darkness1);

        AnimateDarknessConsume(
            _darkness2);

        AnimateDarknessConsume(
            _darkness3);
    }

    private void OnHovered()
    {
        if (_hoverTip == null)
            return;

        NHoverTipSet.Clear();

        var tip =
            NHoverTipSet.CreateAndShow(
                this,
                _hoverTip);

        if (tip != null)
        {
            tip.GlobalPosition =
                GlobalPosition +
                new Vector2(
                    -75f,
                    -500f);

            tip.MouseFilter =
                MouseFilterEnum.Ignore;
        }
    }

    private void OnUnhovered()
    {
        NHoverTipSet.Remove(this);
    }

    public override void _ExitTree()
    {
        NHoverTipSet.Remove(this);

        if (Instance == this)
            Instance = null;

        _display = null;

        _overlay = null;
        _darkMastery = null;

        _darkness1 = null;
        _darkness2 = null;
        _darkness3 = null;

        _anim = null;
        _player = null;
        _hoverTip = null;

        _lastDarkMasteryState = null;
    }
}

[HarmonyPatch(
    typeof(NEnergyCounter),
    nameof(NEnergyCounter._Ready))]
public static class DarknessDisplayOverlayPatch
{
    public static void Postfix(
        NEnergyCounter __instance)
    {
        if (__instance == null)
            return;

        if (!GodotObject.IsInstanceValid(__instance))
            return;

        var state =
            CombatManager.Instance?.DebugOnlyGetState();

        var player =
            state?.Players.FirstOrDefault(
                p => LocalContext.IsMe(p));

        if (player?.Character is not Character.Riku)
            return;

        if (__instance.GetNodeOrNull<DarknessDisplayOverlay>(
                "DarknessDisplayOverlay") != null)
        {
            return;
        }

        __instance.AddChild(
            new DarknessDisplayOverlay
            {
                Name = "DarknessDisplayOverlay"
            });
    }
}
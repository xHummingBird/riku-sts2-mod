using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Riku.RikuCode.Mechanics;

public static class DarkSleightOverlay
{
    private const string NodeName = "DarkSleight_UI";
    private const string ScenePath = "res://Riku/scenes/dark_sleight_display.tscn";

    public static void Ensure(NCard card)
    {
        var model = card.Model;
        var body = card.Body;

        if (model == null || body == null)
            return;

        var node =
            body.GetNodeOrNull<Control>(NodeName);

        if (node == null)
        {
            var scene =
                GD.Load<PackedScene>(ScenePath);

            if (scene == null)
                return;

            node = scene.Instantiate<Control>();
            node.Name = NodeName;
            node.MouseFilter = Control.MouseFilterEnum.Ignore;

            body.AddChild(node);
        }

        bool shouldShow =
            model is IDarkSleight;

        node.Visible = shouldShow;

        node.Position =
            new Vector2(95f, -225f);
    }
}
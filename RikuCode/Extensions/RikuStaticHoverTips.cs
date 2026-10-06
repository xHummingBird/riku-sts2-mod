using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Riku.RikuCode.Extensions;

public class RikuStaticHoverTips
{
    public static readonly IHoverTip SP = new HoverTip(
        new LocString("static_hover_tips", "RIKU_SP.title"),
        new LocString("static_hover_tips", "RIKU_SP.description")
    );
    
    public static readonly IHoverTip Darkness = new HoverTip(
        new LocString("static_hover_tips", "RIKU_DARKNESS.title"),
        new LocString("static_hover_tips", "RIKU_DARKNESS.description")
    );
    
    public static readonly IHoverTip DarkMastery = new HoverTip(
        new LocString("static_hover_tips", "RIKU_DARK_MASTERY.title"),
        new LocString("static_hover_tips", "RIKU_DARK_MASTERY.description")
    );
    
    public static readonly IHoverTip SituationCommand = new HoverTip(
        new LocString("static_hover_tips", "RIKU_SITUATION_COMMAND.title"),
        new LocString("static_hover_tips", "RIKU_SITUATION_COMMAND.description")
    );
    
    public static readonly IHoverTip DarkSleight = new HoverTip(
        new LocString("static_hover_tips", "RIKU_DARK_SLEIGHT.title"),
        new LocString("static_hover_tips", "RIKU_DARK_SLEIGHT.description")
    );
}

/*
 * IHoverTip is built from LocStrings, so a static tip cannot carry a live
 * value the way a card's DynamicVars can. Instead the localized text keeps
 * literal %SP% / %MAXSP% tokens and we fill them in on the tip node once it
 * has been created. Percent tokens are used rather than braces so nothing
 * upstream tries to treat them as SmartFormat placeholders.
 */
public static class RikuHoverTipText
{
    public const string SpToken = "%SP%";

    public const string MaxSpToken = "%MAXSP%";

    private static readonly StringName TextProperty = "text";

    public sealed class TextTarget
    {
        public required GodotObject TextNode { get; init; }

        public required string Template { get; init; }
    }

    /*
     * Snapshots every label under the tip whose text still contains a token,
     * so the values can be re-rendered later without losing the template.
     */
    public static List<TextTarget> CollectTargets(Node root)
    {
        List<TextTarget> targets = [];

        Collect(root, targets);

        return targets;
    }

    public static void Render(
        List<TextTarget> targets,
        int situationPoints,
        int maxSituationPoints)
    {
        foreach (var target in targets)
        {
            if (!GodotObject.IsInstanceValid(target.TextNode))
                continue;

            string text =
                target.Template
                    .Replace(SpToken, situationPoints.ToString())
                    .Replace(MaxSpToken, maxSituationPoints.ToString());

            target.TextNode.Set(TextProperty, text);
        }
    }

    private static void Collect(
        Node root,
        List<TextTarget> targets)
    {
        var value = root.Get(TextProperty);

        if (value.VariantType == Variant.Type.String)
        {
            string text = value.AsString();

            if (text.Contains(SpToken) || text.Contains(MaxSpToken))
            {
                targets.Add(
                    new TextTarget
                    {
                        TextNode = root,
                        Template = text
                    });
            }
        }

        foreach (var child in root.GetChildren())
            Collect(child, targets);
    }
}

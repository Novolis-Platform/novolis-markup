using System.Globalization;
using System.Text;

namespace Novolis.Markup.Html.Css;

/// <summary>Fluent CSS rule / declaration list.</summary>
public sealed class CssRule(string selector)
{
    private readonly List<string> _declarations = new();

    /// <summary>Selector (empty when used for inline styles).</summary>
    public string Selector { get; } = selector;

    /// <summary>Declarations as <c>prop: value;</c> lines (no braces).</summary>
    public string Declarations => string.Join(' ', _declarations);

    /// <summary>Adds a raw declaration.</summary>
    public CssRule Prop(string name, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        _declarations.Add($"{name}: {value};");
        return this;
    }

    /// <summary>Adds a custom property (<c>--name</c>).</summary>
    public CssRule Var(string name, string value)
    {
        var prop = name.StartsWith("--", StringComparison.Ordinal) ? name : $"--{name}";
        return Prop(prop, value);
    }

    // —— Layout ——

    /// <summary>Sets <c>display</c>.</summary>
    public CssRule Display(string value) => Prop("display", value);

    /// <summary>Sets <c>display</c>.</summary>
    public CssRule Display(CssDisplay value) => Display(ToKebab(value.ToString()));

    /// <summary>Sets <c>position</c>.</summary>
    public CssRule Position(string value) => Prop("position", value);

    /// <summary>Sets <c>position</c>.</summary>
    public CssRule Position(CssPosition value) => Position(ToKebab(value.ToString()));

    /// <summary>Sets <c>top</c>.</summary>
    public CssRule Top(string value) => Prop("top", value);

    /// <summary>Sets <c>right</c>.</summary>
    public CssRule Right(string value) => Prop("right", value);

    /// <summary>Sets <c>bottom</c>.</summary>
    public CssRule Bottom(string value) => Prop("bottom", value);

    /// <summary>Sets <c>left</c>.</summary>
    public CssRule Left(string value) => Prop("left", value);

    /// <summary>Sets <c>inset</c>.</summary>
    public CssRule Inset(string value) => Prop("inset", value);

    /// <summary>Sets <c>z-index</c>.</summary>
    public CssRule ZIndex(int value) => Prop("z-index", value.ToString(CultureInfo.InvariantCulture));

    /// <summary>Sets <c>box-sizing</c>.</summary>
    public CssRule BoxSizing(string value) => Prop("box-sizing", value);

    /// <summary>Sets <c>box-sizing: border-box</c>.</summary>
    public CssRule BorderBox() => BoxSizing("border-box");

    /// <summary>Sets <c>overflow</c>.</summary>
    public CssRule Overflow(string value) => Prop("overflow", value);

    /// <summary>Sets <c>overflow-x</c>.</summary>
    public CssRule OverflowX(string value) => Prop("overflow-x", value);

    /// <summary>Sets <c>overflow-y</c>.</summary>
    public CssRule OverflowY(string value) => Prop("overflow-y", value);

    /// <summary>Sets <c>visibility</c>.</summary>
    public CssRule Visibility(string value) => Prop("visibility", value);

    /// <summary>Sets <c>opacity</c>.</summary>
    public CssRule Opacity(double value) => Prop("opacity", value.ToString(CultureInfo.InvariantCulture));

    // —— Size ——

    /// <summary>Sets <c>width</c>.</summary>
    public CssRule Width(string value) => Prop("width", value);

    /// <summary>Sets <c>height</c>.</summary>
    public CssRule Height(string value) => Prop("height", value);

    /// <summary>Sets <c>min-width</c>.</summary>
    public CssRule MinWidth(string value) => Prop("min-width", value);

    /// <summary>Sets <c>min-height</c>.</summary>
    public CssRule MinHeight(string value) => Prop("min-height", value);

    /// <summary>Sets <c>max-width</c>.</summary>
    public CssRule MaxWidth(string value) => Prop("max-width", value);

    /// <summary>Sets <c>max-height</c>.</summary>
    public CssRule MaxHeight(string value) => Prop("max-height", value);

    // —— Spacing ——

    /// <summary>Sets <c>margin</c>.</summary>
    public CssRule Margin(string value) => Prop("margin", value);

    /// <summary>Sets <c>margin</c> in pixels.</summary>
    public CssRule Margin(int px) => Margin($"{px}px");

    /// <summary>Sets <c>margin-top</c>.</summary>
    public CssRule MarginTop(string value) => Prop("margin-top", value);

    /// <summary>Sets <c>margin-right</c>.</summary>
    public CssRule MarginRight(string value) => Prop("margin-right", value);

    /// <summary>Sets <c>margin-bottom</c>.</summary>
    public CssRule MarginBottom(string value) => Prop("margin-bottom", value);

    /// <summary>Sets <c>margin-left</c>.</summary>
    public CssRule MarginLeft(string value) => Prop("margin-left", value);

    /// <summary>Sets <c>padding</c>.</summary>
    public CssRule Padding(string value) => Prop("padding", value);

    /// <summary>Sets <c>padding</c> in pixels.</summary>
    public CssRule Padding(int px) => Padding($"{px}px");

    /// <summary>Sets <c>padding-top</c>.</summary>
    public CssRule PaddingTop(string value) => Prop("padding-top", value);

    /// <summary>Sets <c>padding-right</c>.</summary>
    public CssRule PaddingRight(string value) => Prop("padding-right", value);

    /// <summary>Sets <c>padding-bottom</c>.</summary>
    public CssRule PaddingBottom(string value) => Prop("padding-bottom", value);

    /// <summary>Sets <c>padding-left</c>.</summary>
    public CssRule PaddingLeft(string value) => Prop("padding-left", value);

    /// <summary>Sets <c>gap</c>.</summary>
    public CssRule Gap(string value) => Prop("gap", value);

    /// <summary>Sets <c>row-gap</c>.</summary>
    public CssRule RowGap(string value) => Prop("row-gap", value);

    /// <summary>Sets <c>column-gap</c>.</summary>
    public CssRule ColumnGap(string value) => Prop("column-gap", value);

    // —— Flex ——

    /// <summary>Sets <c>display: flex</c>.</summary>
    public CssRule Flex() => Display(CssDisplay.Flex);

    /// <summary>Sets <c>display: inline-flex</c>.</summary>
    public CssRule InlineFlex() => Display(CssDisplay.InlineFlex);

    /// <summary>Sets <c>flex-direction</c>.</summary>
    public CssRule FlexDirection(string value) => Prop("flex-direction", value);

    /// <summary>Sets <c>flex-wrap</c>.</summary>
    public CssRule FlexWrap(string value) => Prop("flex-wrap", value);

    /// <summary>Sets <c>justify-content</c>.</summary>
    public CssRule JustifyContent(string value) => Prop("justify-content", value);

    /// <summary>Sets <c>align-items</c>.</summary>
    public CssRule AlignItems(string value) => Prop("align-items", value);

    /// <summary>Sets <c>align-self</c>.</summary>
    public CssRule AlignSelf(string value) => Prop("align-self", value);

    /// <summary>Sets <c>flex</c>.</summary>
    public CssRule FlexGrowShrinkBasis(string value) => Prop("flex", value);

    /// <summary>Sets <c>flex-grow</c>.</summary>
    public CssRule FlexGrow(double value) => Prop("flex-grow", value.ToString(CultureInfo.InvariantCulture));

    /// <summary>Sets <c>flex-shrink</c>.</summary>
    public CssRule FlexShrink(double value) => Prop("flex-shrink", value.ToString(CultureInfo.InvariantCulture));

    /// <summary>Sets <c>flex-basis</c>.</summary>
    public CssRule FlexBasis(string value) => Prop("flex-basis", value);

    // —— Grid ——

    /// <summary>Sets <c>display: grid</c>.</summary>
    public CssRule Grid() => Display(CssDisplay.Grid);

    /// <summary>Sets <c>grid-template-columns</c>.</summary>
    public CssRule GridTemplateColumns(string value) => Prop("grid-template-columns", value);

    /// <summary>Sets <c>grid-template-rows</c>.</summary>
    public CssRule GridTemplateRows(string value) => Prop("grid-template-rows", value);

    /// <summary>Sets <c>grid-column</c>.</summary>
    public CssRule GridColumn(string value) => Prop("grid-column", value);

    /// <summary>Sets <c>grid-row</c>.</summary>
    public CssRule GridRow(string value) => Prop("grid-row", value);

    /// <summary>Sets <c>place-items</c>.</summary>
    public CssRule PlaceItems(string value) => Prop("place-items", value);

    // —— Typography ——

    /// <summary>Sets <c>color</c>.</summary>
    public CssRule Color(string value) => Prop("color", value);

    /// <summary>Sets <c>font-family</c>.</summary>
    public CssRule FontFamily(string value) => Prop("font-family", value);

    /// <summary>Sets <c>font-size</c>.</summary>
    public CssRule FontSize(string value) => Prop("font-size", value);

    /// <summary>Sets <c>font-weight</c>.</summary>
    public CssRule FontWeight(string value) => Prop("font-weight", value);

    /// <summary>Sets <c>font-weight</c>.</summary>
    public CssRule FontWeight(int value) => FontWeight(value.ToString(CultureInfo.InvariantCulture));

    /// <summary>Sets <c>font-style</c>.</summary>
    public CssRule FontStyle(string value) => Prop("font-style", value);

    /// <summary>Sets <c>line-height</c>.</summary>
    public CssRule LineHeight(string value) => Prop("line-height", value);

    /// <summary>Sets <c>letter-spacing</c>.</summary>
    public CssRule LetterSpacing(string value) => Prop("letter-spacing", value);

    /// <summary>Sets <c>text-align</c>.</summary>
    public CssRule TextAlign(string value) => Prop("text-align", value);

    /// <summary>Sets <c>text-decoration</c>.</summary>
    public CssRule TextDecoration(string value) => Prop("text-decoration", value);

    /// <summary>Sets <c>text-transform</c>.</summary>
    public CssRule TextTransform(string value) => Prop("text-transform", value);

    /// <summary>Sets <c>white-space</c>.</summary>
    public CssRule WhiteSpace(string value) => Prop("white-space", value);

    /// <summary>Sets <c>word-break</c>.</summary>
    public CssRule WordBreak(string value) => Prop("word-break", value);

    // —— Background / border ——

    /// <summary>Sets <c>background</c>.</summary>
    public CssRule Background(string value) => Prop("background", value);

    /// <summary>Sets <c>background-color</c>.</summary>
    public CssRule BackgroundColor(string value) => Prop("background-color", value);

    /// <summary>Sets <c>background-image</c>.</summary>
    public CssRule BackgroundImage(string value) => Prop("background-image", value);

    /// <summary>Sets <c>background-size</c>.</summary>
    public CssRule BackgroundSize(string value) => Prop("background-size", value);

    /// <summary>Sets <c>background-position</c>.</summary>
    public CssRule BackgroundPosition(string value) => Prop("background-position", value);

    /// <summary>Sets <c>background-repeat</c>.</summary>
    public CssRule BackgroundRepeat(string value) => Prop("background-repeat", value);

    /// <summary>Sets <c>border</c>.</summary>
    public CssRule Border(string value) => Prop("border", value);

    /// <summary>Sets <c>border-top</c>.</summary>
    public CssRule BorderTop(string value) => Prop("border-top", value);

    /// <summary>Sets <c>border-right</c>.</summary>
    public CssRule BorderRight(string value) => Prop("border-right", value);

    /// <summary>Sets <c>border-bottom</c>.</summary>
    public CssRule BorderBottom(string value) => Prop("border-bottom", value);

    /// <summary>Sets <c>border-left</c>.</summary>
    public CssRule BorderLeft(string value) => Prop("border-left", value);

    /// <summary>Sets <c>border-radius</c>.</summary>
    public CssRule BorderRadius(string value) => Prop("border-radius", value);

    /// <summary>Sets <c>border-radius</c> in pixels.</summary>
    public CssRule BorderRadius(int px) => BorderRadius($"{px}px");

    /// <summary>Sets <c>outline</c>.</summary>
    public CssRule Outline(string value) => Prop("outline", value);

    /// <summary>Sets <c>box-shadow</c>.</summary>
    public CssRule BoxShadow(string value) => Prop("box-shadow", value);

    // —— Misc ——

    /// <summary>Sets <c>cursor</c>.</summary>
    public CssRule Cursor(string value) => Prop("cursor", value);

    /// <summary>Sets <c>pointer-events</c>.</summary>
    public CssRule PointerEvents(string value) => Prop("pointer-events", value);

    /// <summary>Sets <c>user-select</c>.</summary>
    public CssRule UserSelect(string value) => Prop("user-select", value);

    /// <summary>Sets <c>object-fit</c>.</summary>
    public CssRule ObjectFit(string value) => Prop("object-fit", value);

    /// <summary>Sets <c>object-position</c>.</summary>
    public CssRule ObjectPosition(string value) => Prop("object-position", value);

    /// <summary>Sets <c>transform</c>.</summary>
    public CssRule Transform(string value) => Prop("transform", value);

    /// <summary>Sets <c>transition</c>.</summary>
    public CssRule Transition(string value) => Prop("transition", value);

    /// <summary>Sets <c>animation</c>.</summary>
    public CssRule Animation(string value) => Prop("animation", value);

    /// <summary>Sets <c>filter</c>.</summary>
    public CssRule Filter(string value) => Prop("filter", value);

    /// <summary>Sets <c>clip-path</c>.</summary>
    public CssRule ClipPath(string value) => Prop("clip-path", value);

    /// <summary>Sets <c>list-style</c>.</summary>
    public CssRule ListStyle(string value) => Prop("list-style", value);

    /// <summary>Sets <c>aspect-ratio</c>.</summary>
    public CssRule AspectRatio(string value) => Prop("aspect-ratio", value);

    /// <inheritdoc />
    public override string ToString()
    {
        if (string.IsNullOrEmpty(Selector))
        {
            return Declarations;
        }

        var sb = new StringBuilder();
        sb.Append(Selector).AppendLine(" {");
        foreach (var declaration in _declarations)
        {
            sb.Append("  ").AppendLine(declaration);
        }

        sb.Append('}');
        return sb.ToString();
    }

    private static string ToKebab(string pascal)
    {
        var sb = new StringBuilder(pascal.Length + 4);
        for (var i = 0; i < pascal.Length; i++)
        {
            var ch = pascal[i];
            if (char.IsUpper(ch) && i > 0)
            {
                sb.Append('-');
            }

            sb.Append(char.ToLowerInvariant(ch));
        }

        return sb.ToString();
    }
}

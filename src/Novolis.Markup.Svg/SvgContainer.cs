namespace Novolis.Markup.Svg;

/// <summary>Base for typed SVG elements that can contain other elements.</summary>
/// <typeparam name="TSelf">Concrete container type used for fluent chaining.</typeparam>
public abstract class SvgContainer<TSelf> : SvgElement
    where TSelf : SvgContainer<TSelf>
{
    /// <summary>Creates a container.</summary>
    internal SvgContainer(string elementName)
        : base(elementName)
    {
    }

    /// <summary>Adds a typed child element.</summary>
    public TSelf Add(SvgElement element)
    {
        AddChild(element);
        return Self;
    }

    /// <summary>Adds a group.</summary>
    public TSelf Group(Action<SvgGroup>? configure = null) =>
        Add(new SvgGroup(), configure);

    /// <summary>Adds a definitions container.</summary>
    public TSelf Defs(Action<SvgDefinitions>? configure = null) =>
        Add(new SvgDefinitions(), configure);

    /// <summary>Adds a circle.</summary>
    public TSelf Circle(SvgPoint center, double radius, Action<SvgCircle>? configure = null) =>
        Add(new SvgCircle(center, radius), configure);

    /// <summary>Adds a circle using numeric coordinates.</summary>
    public TSelf Circle(double centerX, double centerY, double radius, Action<SvgCircle>? configure = null) =>
        Circle(new SvgPoint(centerX, centerY), radius, configure);

    /// <summary>Adds an ellipse.</summary>
    public TSelf Ellipse(
        SvgPoint center,
        double radiusX,
        double radiusY,
        Action<SvgEllipse>? configure = null) =>
        Add(new SvgEllipse(center, radiusX, radiusY), configure);

    /// <summary>Adds a rectangle.</summary>
    public TSelf Rect(SvgRect bounds, Action<SvgRectangle>? configure = null) =>
        Add(new SvgRectangle(bounds), configure);

    /// <summary>Adds a rectangle using numeric coordinates.</summary>
    public TSelf Rect(
        double x,
        double y,
        double width,
        double height,
        Action<SvgRectangle>? configure = null) =>
        Rect(new SvgRect(x, y, width, height), configure);

    /// <summary>Adds a line.</summary>
    public TSelf Line(SvgPoint start, SvgPoint end, Action<SvgLine>? configure = null) =>
        Add(new SvgLine(start, end), configure);

    /// <summary>Adds a line using numeric coordinates.</summary>
    public TSelf Line(
        double x1,
        double y1,
        double x2,
        double y2,
        Action<SvgLine>? configure = null) =>
        Line(new SvgPoint(x1, y1), new SvgPoint(x2, y2), configure);

    /// <summary>Adds a path.</summary>
    public TSelf Path(SvgPathData path, Action<SvgPathElement>? configure = null) =>
        Add(new SvgPathElement(path), configure);

    /// <summary>Adds a polyline.</summary>
    public TSelf Polyline(IEnumerable<SvgPoint> points, Action<SvgPolyline>? configure = null) =>
        Add(new SvgPolyline(points), configure);

    /// <summary>Adds a polygon.</summary>
    public TSelf Polygon(IEnumerable<SvgPoint> points, Action<SvgPolygon>? configure = null) =>
        Add(new SvgPolygon(points), configure);

    /// <summary>Adds a text element.</summary>
    public TSelf Text(SvgPoint origin, string content, Action<SvgTextElement>? configure = null) =>
        Add(new SvgTextElement(origin, content), configure);

    /// <summary>Adds an image.</summary>
    public TSelf Image(SvgRect bounds, Uri href, Action<SvgImage>? configure = null) =>
        Add(new SvgImage(bounds, href), configure);

    /// <summary>Adds a reference to an element in this document.</summary>
    public TSelf Use(SvgReference reference, Action<SvgUse>? configure = null) =>
        Add(new SvgUse(reference), configure);

    /// <summary>Adds a reference using an element id.</summary>
    public TSelf Use(string id, Action<SvgUse>? configure = null) =>
        Use(new SvgReference(id), configure);

    /// <summary>Adds a title.</summary>
    public TSelf Title(string title) => Add(new SvgTitleElement(title));

    /// <summary>Adds a description.</summary>
    public TSelf Description(string description) => Add(new SvgDescriptionElement(description));

    /// <summary>Adds a linear gradient definition.</summary>
    public TSelf LinearGradient(string id, Action<SvgLinearGradient>? configure = null) =>
        Add(new SvgLinearGradient(id), configure);

    /// <summary>Adds a radial gradient definition.</summary>
    public TSelf RadialGradient(string id, Action<SvgRadialGradient>? configure = null) =>
        Add(new SvgRadialGradient(id), configure);

    /// <summary>Adds a marker definition.</summary>
    public TSelf Marker(string id, Action<SvgMarker>? configure = null) =>
        Add(new SvgMarker(id), configure);

    /// <summary>Sets the element id and keeps fluent container chaining.</summary>
    public new TSelf Id(string id) => (TSelf)base.Id(id);

    /// <summary>Sets CSS classes and keeps fluent container chaining.</summary>
    public new TSelf Class(params string[] classes) => (TSelf)base.Class(classes);

    /// <summary>Sets the fill paint and keeps fluent container chaining.</summary>
    public new TSelf Fill(SvgPaint paint) => (TSelf)base.Fill(paint);

    /// <summary>Sets the stroke paint and keeps fluent container chaining.</summary>
    public new TSelf Stroke(SvgPaint paint) => (TSelf)base.Stroke(paint);

    /// <summary>Sets stroke width and keeps fluent container chaining.</summary>
    public new TSelf StrokeWidth(double width) => (TSelf)base.StrokeWidth(width);

    /// <summary>Sets stroke line cap and keeps fluent container chaining.</summary>
    public new TSelf StrokeLineCap(SvgLineCap lineCap) => (TSelf)base.StrokeLineCap(lineCap);

    /// <summary>Sets stroke line join and keeps fluent container chaining.</summary>
    public new TSelf StrokeLineJoin(SvgLineJoin lineJoin) => (TSelf)base.StrokeLineJoin(lineJoin);

    /// <summary>Sets a stroke dash pattern and keeps fluent container chaining.</summary>
    public new TSelf StrokeDashArray(IEnumerable<double> values) => (TSelf)base.StrokeDashArray(values);

    /// <summary>Sets fill rule and keeps fluent container chaining.</summary>
    public new TSelf FillRule(SvgFillRule fillRule) => (TSelf)base.FillRule(fillRule);

    /// <summary>Sets clipping rule and keeps fluent container chaining.</summary>
    public new TSelf ClipRule(SvgFillRule clipRule) => (TSelf)base.ClipRule(clipRule);

    /// <summary>Sets opacity and keeps fluent container chaining.</summary>
    public new TSelf Opacity(double opacity) => (TSelf)base.Opacity(opacity);

    /// <summary>Sets fill opacity and keeps fluent container chaining.</summary>
    public new TSelf FillOpacity(double opacity) => (TSelf)base.FillOpacity(opacity);

    /// <summary>Sets stroke opacity and keeps fluent container chaining.</summary>
    public new TSelf StrokeOpacity(double opacity) => (TSelf)base.StrokeOpacity(opacity);

    /// <summary>Sets a transform and keeps fluent container chaining.</summary>
    public new TSelf Transform(SvgTransformList transforms) => (TSelf)base.Transform(transforms);

    /// <summary>Sets inline CSS and keeps fluent container chaining.</summary>
    public new TSelf Style(string style) => (TSelf)base.Style(style);

    /// <summary>Sets an accessibility label and keeps fluent container chaining.</summary>
    public new TSelf AriaLabel(string label) => (TSelf)base.AriaLabel(label);

    /// <summary>Sets an ARIA role and keeps fluent container chaining.</summary>
    public new TSelf Role(string role) => (TSelf)base.Role(role);

    /// <summary>Sets a data attribute and keeps fluent container chaining.</summary>
    public new TSelf Data(string name, string value) => (TSelf)base.Data(name, value);

    /// <summary>Sets font family and keeps fluent container chaining.</summary>
    public new TSelf FontFamily(string fontFamily) => (TSelf)base.FontFamily(fontFamily);

    /// <summary>Sets font size and keeps fluent container chaining.</summary>
    public new TSelf FontSize(SvgLength fontSize) => (TSelf)base.FontSize(fontSize);

    /// <summary>Sets text anchor and keeps fluent container chaining.</summary>
    public new TSelf TextAnchor(SvgTextAnchor anchor) => (TSelf)base.TextAnchor(anchor);

    /// <summary>Adds a marker at the beginning of a stroke and keeps fluent container chaining.</summary>
    public new TSelf MarkerStart(SvgReference reference) => (TSelf)base.MarkerStart(reference);

    /// <summary>Adds a marker in the middle of a stroke and keeps fluent container chaining.</summary>
    public new TSelf MarkerMid(SvgReference reference) => (TSelf)base.MarkerMid(reference);

    /// <summary>Adds a marker at the end of a stroke and keeps fluent container chaining.</summary>
    public new TSelf MarkerEnd(SvgReference reference) => (TSelf)base.MarkerEnd(reference);

    private TSelf Self => (TSelf)this;

    private TSelf Add<TElement>(TElement element, Action<TElement>? configure)
        where TElement : SvgElement
    {
        configure?.Invoke(element);
        return Add(element);
    }
}

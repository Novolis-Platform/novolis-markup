using System.Xml;

namespace Novolis.Markup.Svg;

/// <summary>Base class for typed SVG elements.</summary>
public abstract class SvgElement
{
    private readonly List<(string Name, string Value)> _attributes = [];
    private readonly List<SvgElement> _children = [];

    /// <summary>Creates an SVG element.</summary>
    internal SvgElement(string elementName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(elementName);
        ElementName = elementName;
    }

    /// <summary>Element name used during serialization.</summary>
    protected string ElementName { get; }

    /// <summary>Sets the element id.</summary>
    public SvgElement Id(string id)
    {
        return Set("id", new SvgReference(id).Id);
    }

    /// <summary>Sets one or more CSS classes.</summary>
    public SvgElement Class(params string[] classes)
    {
        ArgumentNullException.ThrowIfNull(classes);
        var value = string.Join(
            ' ',
            classes.Where(static value => !string.IsNullOrWhiteSpace(value))
                .Select(static value => value.Trim()));
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return Set("class", value);
    }

    /// <summary>Sets the fill paint.</summary>
    public SvgElement Fill(SvgPaint paint) => Set("fill", paint.Value);

    /// <summary>Sets the stroke paint.</summary>
    public SvgElement Stroke(SvgPaint paint) => Set("stroke", paint.Value);

    /// <summary>Sets the stroke width.</summary>
    public SvgElement StrokeWidth(double width)
    {
        SvgNumber.EnsureNonNegative(width, nameof(width));
        return Set("stroke-width", SvgNumber.Format(width));
    }

    /// <summary>Sets the stroke line cap.</summary>
    public SvgElement StrokeLineCap(SvgLineCap lineCap) =>
        Set("stroke-linecap", lineCap switch
        {
            SvgLineCap.Butt => "butt",
            SvgLineCap.Round => "round",
            SvgLineCap.Square => "square",
            _ => throw new ArgumentOutOfRangeException(nameof(lineCap)),
        });

    /// <summary>Sets the stroke line join.</summary>
    public SvgElement StrokeLineJoin(SvgLineJoin lineJoin) =>
        Set("stroke-linejoin", lineJoin switch
        {
            SvgLineJoin.Miter => "miter",
            SvgLineJoin.Round => "round",
            SvgLineJoin.Bevel => "bevel",
            _ => throw new ArgumentOutOfRangeException(nameof(lineJoin)),
        });

    /// <summary>Sets a stroke dash pattern.</summary>
    public SvgElement StrokeDashArray(IEnumerable<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var numbers = values.ToArray();
        if (numbers.Length == 0)
        {
            throw new ArgumentException("A dash pattern must contain at least one value.", nameof(values));
        }

        foreach (var value in numbers)
        {
            SvgNumber.EnsureNonNegative(value, nameof(values));
        }

        return Set("stroke-dasharray", string.Join(' ', numbers.Select(SvgNumber.Format)));
    }

    /// <summary>Sets the fill rule.</summary>
    public SvgElement FillRule(SvgFillRule fillRule) =>
        Set("fill-rule", fillRule switch
        {
            SvgFillRule.NonZero => "nonzero",
            SvgFillRule.EvenOdd => "evenodd",
            _ => throw new ArgumentOutOfRangeException(nameof(fillRule)),
        });

    /// <summary>Sets the clipping rule.</summary>
    public SvgElement ClipRule(SvgFillRule clipRule) =>
        Set("clip-rule", clipRule switch
        {
            SvgFillRule.NonZero => "nonzero",
            SvgFillRule.EvenOdd => "evenodd",
            _ => throw new ArgumentOutOfRangeException(nameof(clipRule)),
        });

    /// <summary>Sets element opacity.</summary>
    public SvgElement Opacity(double opacity) => SetUnitInterval("opacity", opacity);

    /// <summary>Sets fill opacity.</summary>
    public SvgElement FillOpacity(double opacity) => SetUnitInterval("fill-opacity", opacity);

    /// <summary>Sets stroke opacity.</summary>
    public SvgElement StrokeOpacity(double opacity) => SetUnitInterval("stroke-opacity", opacity);

    /// <summary>Sets a transform list.</summary>
    public SvgElement Transform(SvgTransformList transforms)
    {
        ArgumentNullException.ThrowIfNull(transforms);
        if (transforms.IsEmpty)
        {
            throw new ArgumentException("The transform list must contain at least one transform.", nameof(transforms));
        }

        return Set("transform", transforms.ToString());
    }

    /// <summary>Sets inline CSS declarations.</summary>
    public SvgElement Style(string style)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(style);
        return Set("style", style);
    }

    /// <summary>Sets an accessibility label.</summary>
    public SvgElement AriaLabel(string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        return Set("aria-label", label);
    }

    /// <summary>Sets an ARIA role.</summary>
    public SvgElement Role(string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        return Set("role", role);
    }

    /// <summary>Sets a data attribute.</summary>
    public SvgElement Data(string name, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (name.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException("A data attribute name must not contain whitespace.", nameof(name));
        }

        return Set($"data-{name}", value);
    }

    /// <summary>Sets the font family.</summary>
    public SvgElement FontFamily(string fontFamily)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fontFamily);
        return Set("font-family", fontFamily);
    }

    /// <summary>Sets the font size.</summary>
    public SvgElement FontSize(SvgLength fontSize) => Set("font-size", fontSize.ToString());

    /// <summary>Sets the text anchor.</summary>
    public SvgElement TextAnchor(SvgTextAnchor anchor) =>
        Set("text-anchor", anchor switch
        {
            SvgTextAnchor.Start => "start",
            SvgTextAnchor.Middle => "middle",
            SvgTextAnchor.End => "end",
            _ => throw new ArgumentOutOfRangeException(nameof(anchor)),
        });

    /// <summary>Sets a marker at the beginning of a stroke.</summary>
    public SvgElement MarkerStart(SvgReference reference) => Set("marker-start", SvgPaint.Url(reference.Id).Value);

    /// <summary>Sets a marker in the middle of a stroke.</summary>
    public SvgElement MarkerMid(SvgReference reference) => Set("marker-mid", SvgPaint.Url(reference.Id).Value);

    /// <summary>Sets a marker at the end of a stroke.</summary>
    public SvgElement MarkerEnd(SvgReference reference) => Set("marker-end", SvgPaint.Url(reference.Id).Value);

    /// <summary>Serializes this element as SVG.</summary>
    public override string ToString() => SvgSerialization.Serialize(this);

    internal SvgElement SetAttribute(string name, string value) => Set(name, value);

    internal void AddChild(SvgElement child)
    {
        ArgumentNullException.ThrowIfNull(child);
        _children.Add(child);
    }

    internal void WriteTo(XmlWriter writer)
    {
        writer.WriteStartElement(ElementName, SvgDocument.Namespace);
        foreach (var (name, value) in _attributes)
        {
            writer.WriteAttributeString(name, value);
        }

        WriteContent(writer);
        foreach (var child in _children)
        {
            child.WriteTo(writer);
        }

        writer.WriteEndElement();
    }

    internal virtual void WriteContent(XmlWriter writer)
    {
    }

    private SvgElement Set(string name, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(value);
        for (var index = 0; index < _attributes.Count; index++)
        {
            if (string.Equals(_attributes[index].Name, name, StringComparison.Ordinal))
            {
                _attributes[index] = (name, value);
                return this;
            }
        }

        _attributes.Add((name, value));
        return this;
    }

    private SvgElement SetUnitInterval(string name, double value)
    {
        SvgNumber.EnsureFinite(value, nameof(value));
        if (value is < 0d or > 1d)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Opacity must be between 0 and 1.");
        }

        return Set(name, SvgNumber.Format(value));
    }
}

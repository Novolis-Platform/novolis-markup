namespace Novolis.Markup.Svg;

/// <summary>A typed SVG marker definition.</summary>
public sealed class SvgMarker : SvgContainer<SvgMarker>
{
    /// <summary>Creates a marker definition.</summary>
    public SvgMarker(string id)
        : base("marker")
    {
        SetAttribute("id", new SvgReference(id).Id);
    }

    /// <summary>Sets the marker width.</summary>
    public SvgMarker MarkerWidth(SvgLength width)
    {
        width.EnsureNonNegative(nameof(width));
        SetAttribute("markerWidth", width.ToString());
        return this;
    }

    /// <summary>Sets the marker height.</summary>
    public SvgMarker MarkerHeight(SvgLength height)
    {
        height.EnsureNonNegative(nameof(height));
        SetAttribute("markerHeight", height.ToString());
        return this;
    }

    /// <summary>Sets the marker reference x coordinate.</summary>
    public SvgMarker RefX(SvgLength x)
    {
        SetAttribute("refX", x.ToString());
        return this;
    }

    /// <summary>Sets the marker reference y coordinate.</summary>
    public SvgMarker RefY(SvgLength y)
    {
        SetAttribute("refY", y.ToString());
        return this;
    }

    /// <summary>Sets marker orientation to automatic.</summary>
    public SvgMarker OrientAuto()
    {
        SetAttribute("orient", "auto");
        return this;
    }

    /// <summary>Sets marker orientation to a fixed angle.</summary>
    public SvgMarker Orient(double degrees)
    {
        SetAttribute("orient", SvgNumber.Format(degrees));
        return this;
    }

    /// <summary>Sets the marker viewBox.</summary>
    public SvgMarker ViewBox(SvgViewBox viewBox)
    {
        SetAttribute("viewBox", viewBox.Serialize());
        return this;
    }
}

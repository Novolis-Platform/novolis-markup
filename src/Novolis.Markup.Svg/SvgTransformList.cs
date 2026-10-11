using System.Text;

namespace Novolis.Markup.Svg;

/// <summary>A fluent list of validated SVG transforms.</summary>
public sealed class SvgTransformList
{
    private readonly List<string> _transforms = [];

    /// <summary>True when no transforms have been added.</summary>
    public bool IsEmpty => _transforms.Count == 0;

    /// <summary>Adds a translation.</summary>
    public SvgTransformList Translate(double x, double y)
    {
        _transforms.Add($"translate({SvgNumber.Format(x)} {SvgNumber.Format(y)})");
        return this;
    }

    /// <summary>Adds a uniform scale.</summary>
    public SvgTransformList Scale(double scale)
    {
        _transforms.Add($"scale({SvgNumber.Format(scale)})");
        return this;
    }

    /// <summary>Adds a non-uniform scale.</summary>
    public SvgTransformList Scale(double x, double y)
    {
        _transforms.Add($"scale({SvgNumber.Format(x)} {SvgNumber.Format(y)})");
        return this;
    }

    /// <summary>Adds a rotation around the origin.</summary>
    public SvgTransformList Rotate(double degrees)
    {
        _transforms.Add($"rotate({SvgNumber.Format(degrees)})");
        return this;
    }

    /// <summary>Adds a rotation around a point.</summary>
    public SvgTransformList Rotate(double degrees, SvgPoint center)
    {
        _transforms.Add(
            $"rotate({SvgNumber.Format(degrees)} {SvgNumber.Format(center.X)} {SvgNumber.Format(center.Y)})");
        return this;
    }

    /// <summary>Adds an x-axis skew.</summary>
    public SvgTransformList SkewX(double degrees)
    {
        _transforms.Add($"skewX({SvgNumber.Format(degrees)})");
        return this;
    }

    /// <summary>Adds a y-axis skew.</summary>
    public SvgTransformList SkewY(double degrees)
    {
        _transforms.Add($"skewY({SvgNumber.Format(degrees)})");
        return this;
    }

    internal string Serialize()
    {
        var builder = new StringBuilder();
        for (var i = 0; i < _transforms.Count; i++)
        {
            if (i > 0)
            {
                builder.Append(' ');
            }

            builder.Append(_transforms[i]);
        }

        return builder.ToString();
    }

    /// <inheritdoc />
    public override string ToString() => Serialize();
}

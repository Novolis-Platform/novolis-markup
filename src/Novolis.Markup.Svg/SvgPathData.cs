using System.Text;

namespace Novolis.Markup.Svg;

/// <summary>A fluent, typed SVG path-data builder.</summary>
public sealed class SvgPathData
{
    private readonly StringBuilder _path = new();

    /// <summary>True when no path commands have been added.</summary>
    public bool IsEmpty => _path.Length == 0;

    /// <summary>Adds an absolute move command.</summary>
    public SvgPathData MoveTo(SvgPoint point) => Command("M", Pair(point));

    /// <summary>Adds an absolute line command.</summary>
    public SvgPathData LineTo(SvgPoint point) => Command("L", Pair(point));

    /// <summary>Adds an absolute horizontal line command.</summary>
    public SvgPathData HorizontalTo(double x) => Command("H", SvgNumber.Format(x));

    /// <summary>Adds an absolute vertical line command.</summary>
    public SvgPathData VerticalTo(double y) => Command("V", SvgNumber.Format(y));

    /// <summary>Adds an absolute cubic Bézier command.</summary>
    public SvgPathData CubicTo(SvgPoint control1, SvgPoint control2, SvgPoint point) =>
        Command("C", Pair(control1), Pair(control2), Pair(point));

    /// <summary>Adds an absolute quadratic Bézier command.</summary>
    public SvgPathData QuadraticTo(SvgPoint control, SvgPoint point) =>
        Command("Q", Pair(control), Pair(point));

    /// <summary>Adds an absolute elliptical arc command.</summary>
    public SvgPathData ArcTo(
        double radiusX,
        double radiusY,
        double rotationDegrees,
        bool largeArc,
        bool sweep,
        SvgPoint point)
    {
        SvgNumber.EnsureNonNegative(radiusX, nameof(radiusX));
        SvgNumber.EnsureNonNegative(radiusY, nameof(radiusY));
        return Command(
            "A",
            SvgNumber.Format(radiusX),
            SvgNumber.Format(radiusY),
            SvgNumber.Format(rotationDegrees),
            largeArc ? "1" : "0",
            sweep ? "1" : "0",
            Pair(point));
    }

    /// <summary>Closes the current subpath.</summary>
    public SvgPathData Close()
    {
        if (_path.Length > 0)
        {
            _path.Append(" Z");
        }

        return this;
    }

    /// <inheritdoc />
    public override string ToString() => _path.ToString();

    private SvgPathData Command(string command, params string[] values)
    {
        if (_path.Length > 0)
        {
            _path.Append(' ');
        }

        _path.Append(command);
        if (values.Length > 0)
        {
            _path.Append(' ');
            _path.AppendJoin(' ', values);
        }

        return this;
    }

    private static string Pair(SvgPoint point) =>
        $"{SvgNumber.Format(point.X)} {SvgNumber.Format(point.Y)}";
}

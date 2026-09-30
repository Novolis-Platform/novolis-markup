namespace Novolis.Markup.Mermaid;

/// <summary>Shape delimiter helpers.</summary>
public static class ShapeExtensions
{
    /// <summary>Wraps a label in the Mermaid shape delimiters for this shape.</summary>
    public static string Wrap(this Shape shape, string label) => shape switch
    {
        Shape.Circle => $"(({label}))",
        Shape.DoubleCircle => $"((({label})))",
        Shape.Subroutine => $"[[{label}]]",
        Shape.Rounded => $"({label})",
        Shape.Stadium => $"([{label}])",
        Shape.Hexagon => $"{{{{{label}}}}}",
        Shape.Database => $"[({label})]",
        Shape.Diamond or Shape.Rhombus => $"{{{label}}}",
        Shape.Parallelogram => $"[/{label}/]",
        Shape.ParallelogramAlt => $"[\\{label}\\]",
        Shape.Asymmetric => $">{label}]",
        Shape.Trapezoid => $"[/{label}\\]",
        Shape.TrapezoidAlt => $"[\\{label}/]",
        _ => $"[{label}]",
    };

    /// <summary>Parses a shape wrapper such as <c>[A]</c> or <c>((A))</c>.</summary>
    public static bool TryUnwrap(string token, out string label, out Shape shape)
    {
        label = token;
        shape = Shape.Rectangle;
        if (token.Length < 2)
            return false;

        if (token.StartsWith("(((", StringComparison.Ordinal) && token.EndsWith(")))", StringComparison.Ordinal))
        {
            label = token[3..^3];
            shape = Shape.DoubleCircle;
            return true;
        }

        if (token.StartsWith("((", StringComparison.Ordinal) && token.EndsWith("))", StringComparison.Ordinal))
        {
            label = token[2..^2];
            shape = Shape.Circle;
            return true;
        }

        if (token.StartsWith("[[", StringComparison.Ordinal) && token.EndsWith("]]", StringComparison.Ordinal))
        {
            label = token[2..^2];
            shape = Shape.Subroutine;
            return true;
        }

        if (token.StartsWith("{{", StringComparison.Ordinal) && token.EndsWith("}}", StringComparison.Ordinal))
        {
            label = token[2..^2];
            shape = Shape.Hexagon;
            return true;
        }

        if (token.StartsWith("([", StringComparison.Ordinal) && token.EndsWith("])", StringComparison.Ordinal))
        {
            label = token[2..^2];
            shape = Shape.Stadium;
            return true;
        }

        if (token.StartsWith("[(", StringComparison.Ordinal) && token.EndsWith(")]", StringComparison.Ordinal))
        {
            label = token[2..^2];
            shape = Shape.Database;
            return true;
        }

        if (token.StartsWith("(/", StringComparison.Ordinal))
        {
            // not used
        }

        if (token.StartsWith("[/", StringComparison.Ordinal) && token.EndsWith("/]", StringComparison.Ordinal))
        {
            label = token[2..^2];
            shape = Shape.Parallelogram;
            return true;
        }

        if (token.StartsWith("[/", StringComparison.Ordinal) && token.EndsWith("\\]", StringComparison.Ordinal))
        {
            label = token[2..^2];
            shape = Shape.Trapezoid;
            return true;
        }

        if (token.StartsWith("[\\", StringComparison.Ordinal) && token.EndsWith("/]", StringComparison.Ordinal))
        {
            label = token[2..^2];
            shape = Shape.TrapezoidAlt;
            return true;
        }

        if (token.StartsWith("[\\", StringComparison.Ordinal) && token.EndsWith("\\]", StringComparison.Ordinal))
        {
            label = token[2..^2];
            shape = Shape.ParallelogramAlt;
            return true;
        }

        if (token.StartsWith(">", StringComparison.Ordinal) && token.EndsWith("]", StringComparison.Ordinal))
        {
            label = token[1..^1];
            shape = Shape.Asymmetric;
            return true;
        }

        if (token.StartsWith('(') && token.EndsWith(')'))
        {
            label = token[1..^1];
            shape = Shape.Rounded;
            return true;
        }

        if (token.StartsWith('{') && token.EndsWith('}'))
        {
            label = token[1..^1];
            shape = Shape.Diamond;
            return true;
        }

        if (token.StartsWith('[') && token.EndsWith(']'))
        {
            label = token[1..^1];
            shape = Shape.Rectangle;
            return true;
        }

        return false;
    }
}

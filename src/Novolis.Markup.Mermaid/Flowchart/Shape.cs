namespace Novolis.Markup.Mermaid;

/// <summary>Flowchart node shape wrappers.</summary>
public enum Shape
{
    /// <summary>Rectangle <c>[text]</c>.</summary>
    Rectangle,

    /// <summary>Rounded rectangle <c>(text)</c>.</summary>
    Rounded,

    /// <summary>Circle <c>((text))</c>.</summary>
    Circle,

    /// <summary>Double circle <c>(((text)))</c>.</summary>
    DoubleCircle,

    /// <summary>Stadium / pill <c>([text])</c>.</summary>
    Stadium,

    /// <summary>Subroutine <c>[[text]]</c>.</summary>
    Subroutine,

    /// <summary>Cylinder / database <c>[(text)]</c>.</summary>
    Database,

    /// <summary>Diamond / rhombus <c>{text}</c>.</summary>
    Diamond,

    /// <summary>Hexagon <c>{{text}}</c>.</summary>
    Hexagon,

    /// <summary>Parallelogram <c>[/text/]</c>.</summary>
    Parallelogram,

    /// <summary>Alt parallelogram <c>[\text\]</c>.</summary>
    ParallelogramAlt,

    /// <summary>Asymmetric <c>&gt;text]</c>.</summary>
    Asymmetric,

    /// <summary>Rhombus alias of <see cref="Diamond"/>.</summary>
    Rhombus,

    /// <summary>Trapezoid <c>[/text\]</c>.</summary>
    Trapezoid,

    /// <summary>Alt trapezoid <c>[\text/]</c>.</summary>
    TrapezoidAlt,
}

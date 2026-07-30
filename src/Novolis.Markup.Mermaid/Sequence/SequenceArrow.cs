namespace Novolis.Markup.Mermaid;

/// <summary>Sequence message arrow styles.</summary>
public enum SequenceArrow
{
    /// <summary>Solid arrow <c>-&gt;&gt;</c>.</summary>
    Solid,

    /// <summary>Dotted arrow <c>--&gt;&gt;</c>.</summary>
    Dotted,

    /// <summary>Solid open arrow <c>-&gt;</c>.</summary>
    SolidOpen,

    /// <summary>Dotted open arrow <c>--&gt;</c>.</summary>
    DottedOpen,

    /// <summary>Solid cross <c>-x</c>.</summary>
    SolidCross,

    /// <summary>Dotted cross <c>--x</c>.</summary>
    DottedCross,

    /// <summary>Solid async <c>-)</c>.</summary>
    SolidAsync,

    /// <summary>Dotted async <c>--)</c>.</summary>
    DottedAsync,
}

/// <summary>Extensions for <see cref="SequenceArrow"/>.</summary>
public static class SequenceArrowExtensions
{
    /// <summary>Returns the Mermaid token for the arrow.</summary>
    public static string ToToken(this SequenceArrow arrow) => arrow switch
    {
        SequenceArrow.Solid => "->>",
        SequenceArrow.Dotted => "-->>",
        SequenceArrow.SolidOpen => "->",
        SequenceArrow.DottedOpen => "-->",
        SequenceArrow.SolidCross => "-x",
        SequenceArrow.DottedCross => "--x",
        SequenceArrow.SolidAsync => "-)",
        SequenceArrow.DottedAsync => "--)",
        _ => "->>",
    };
}

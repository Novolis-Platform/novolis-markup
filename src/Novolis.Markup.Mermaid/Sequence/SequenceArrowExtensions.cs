namespace Novolis.Markup.Mermaid;

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

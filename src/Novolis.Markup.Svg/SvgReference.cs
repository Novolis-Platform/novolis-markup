namespace Novolis.Markup.Svg;

/// <summary>A reference to an element identified inside the SVG document.</summary>
public readonly record struct SvgReference
{
    /// <summary>Creates an internal element reference.</summary>
    public SvgReference(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        id = id.Trim().TrimStart('#');
        if (id.Length == 0 || id.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException("An SVG reference must contain an element id.", nameof(id));
        }

        Id = id;
    }

    /// <summary>Referenced element id.</summary>
    public string Id { get; }

    /// <summary>Creates an internal SVG reference.</summary>
    public static SvgReference To(string id) => new(id);

    /// <inheritdoc />
    public override string ToString() => $"#{Id}";
}

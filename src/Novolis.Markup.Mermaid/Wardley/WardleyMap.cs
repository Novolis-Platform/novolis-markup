namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>wardley-beta</c> maps.</summary>
public sealed class WardleyMap : IMermaidable
{
    readonly List<string> _lines = [];
    string? _title;
    (int Width, int Height)? _size;

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Sets the map title.</summary>
    public WardleyMap Title(string title)
    {
        _title = title;
        return this;
    }

    /// <summary>Sets canvas size in pixels.</summary>
    public WardleyMap Size(int width, int height)
    {
        _size = (width, height);
        return this;
    }

    /// <summary>Adds a component at <c>[visibility, evolution]</c> (OWM coordinates).</summary>
    public WardleyMap Component(string name, double visibility, double evolution, string? decorator = null)
    {
        var line = $"component {name} [{Format(visibility)}, {Format(evolution)}]";
        if (!string.IsNullOrWhiteSpace(decorator))
            line += $" ({decorator})";
        _lines.Add(line);
        return this;
    }

    /// <summary>Adds an anchor (user/need) at <c>[visibility, evolution]</c>.</summary>
    public WardleyMap Anchor(string name, double visibility, double evolution)
    {
        _lines.Add($"anchor {name} [{Format(visibility)}, {Format(evolution)}]");
        return this;
    }

    /// <summary>Adds a dependency <c>A -&gt; B</c>.</summary>
    public WardleyMap Link(string from, string to)
    {
        _lines.Add($"{from} -> {to}");
        return this;
    }

    /// <summary>Adds a flow <c>A +&gt; B</c>.</summary>
    public WardleyMap Flow(string from, string to)
    {
        _lines.Add($"{from} +> {to}");
        return this;
    }

    /// <summary>Marks a component as evolving toward a new evolution value.</summary>
    public WardleyMap Evolve(string name, double targetEvolution)
    {
        _lines.Add($"evolve {name} {Format(targetEvolution)}");
        return this;
    }

    /// <summary>Places a note on the map.</summary>
    public WardleyMap Note(string text, double visibility, double evolution)
    {
        _lines.Add($"note \"{text.Replace("\"", "#quot;", StringComparison.Ordinal)}\" [{Format(visibility)}, {Format(evolution)}]");
        return this;
    }

    /// <summary>Appends a raw statement (pipelines, annotations, custom evolution stages).</summary>
    public WardleyMap AddStatement(string statement)
    {
        _lines.Add(statement);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("wardley-beta");
        writer.IncreaseIndent();
        if (!string.IsNullOrWhiteSpace(_title))
            writer.WriteLine("title {0}", _title);
        if (_size is { } size)
            writer.WriteLine("size [{0}, {1}]", size.Width, size.Height);
        foreach (var line in _lines)
            writer.WriteLine(line);
        writer.DecreaseIndent();
        return writer;
    }

    static string Format(double value) => value.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
}

namespace Novolis.Markup.Mermaid;

/// <summary>C4 diagram kinds supported by Mermaid.</summary>
public enum C4Kind
{
    /// <summary><c>C4Context</c>.</summary>
    Context,

    /// <summary><c>C4Container</c>.</summary>
    Container,

    /// <summary><c>C4Component</c>.</summary>
    Component,

    /// <summary><c>C4Dynamic</c>.</summary>
    Dynamic,

    /// <summary><c>C4Deployment</c>.</summary>
    Deployment,
}

/// <summary>Fluent builder for Mermaid C4 diagrams.</summary>
public sealed class C4Diagram(C4Kind kind, string title) : IMermaidable
{
    private readonly List<string> _lines = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a person.</summary>
    public C4Diagram Person(string alias, string label, string? descr = null)
    {
        _lines.Add(FormatCall("Person", alias, label, descr));
        return this;
    }

    /// <summary>Adds a system.</summary>
    public C4Diagram System(string alias, string label, string? descr = null)
    {
        _lines.Add(FormatCall("System", alias, label, descr));
        return this;
    }

    /// <summary>Adds an external system.</summary>
    public C4Diagram System_Ext(string alias, string label, string? descr = null)
    {
        _lines.Add(FormatCall("System_Ext", alias, label, descr));
        return this;
    }

    /// <summary>Adds a container.</summary>
    public C4Diagram Container(string alias, string label, string technology, string? descr = null)
    {
        _lines.Add(string.IsNullOrWhiteSpace(descr)
            ? $"Container({alias}, \"{label}\", \"{technology}\")"
            : $"Container({alias}, \"{label}\", \"{technology}\", \"{descr}\")");
        return this;
    }

    /// <summary>Adds a Rel between aliases.</summary>
    public C4Diagram Rel(string from, string to, string label, string? technology = null)
    {
        _lines.Add(string.IsNullOrWhiteSpace(technology)
            ? $"Rel({from}, {to}, \"{label}\")"
            : $"Rel({from}, {to}, \"{label}\", \"{technology}\")");
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var header = kind switch
        {
            C4Kind.Context => "C4Context",
            C4Kind.Container => "C4Container",
            C4Kind.Component => "C4Component",
            C4Kind.Dynamic => "C4Dynamic",
            C4Kind.Deployment => "C4Deployment",
            _ => "C4Context",
        };

        var writer = new IndentedStringBuilder();
        writer.WriteLine(header);
        writer.IncreaseIndent();
        writer.WriteLine("title {0}", title);
        foreach (var line in _lines)
            writer.WriteLine(line);
        writer.DecreaseIndent();
        return writer;
    }

    private static string FormatCall(string fn, string alias, string label, string? descr) =>
        string.IsNullOrWhiteSpace(descr)
            ? $"{fn}({alias}, \"{label}\")"
            : $"{fn}({alias}, \"{label}\", \"{descr}\")";
}

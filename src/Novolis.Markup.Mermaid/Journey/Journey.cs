namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid user <c>journey</c> diagrams.</summary>
public sealed class Journey(string title) : IMermaidable
{
    private readonly List<JourneySection> _sections = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a journey section.</summary>
    public Journey AddSection(JourneySection section)
    {
        _sections.Add(section);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("journey");
        writer.IncreaseIndent();
        writer.WriteLine("title {0}", title);
        foreach (var section in _sections)
            writer.WriteLine(section.GetBuilder());
        writer.DecreaseIndent();
        return writer;
    }
}

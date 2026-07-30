namespace Novolis.Markup.Mermaid;

/// <summary>A scored journey task with actors.</summary>
public sealed class JourneyTask(string label, int score, params string[] actors) : IMermaidable
{
    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("{0}: {1}: {2}", label, score, string.Join(", ", actors));
        return writer;
    }
}

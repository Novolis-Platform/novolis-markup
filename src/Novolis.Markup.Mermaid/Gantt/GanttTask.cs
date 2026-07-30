namespace Novolis.Markup.Mermaid;

/// <summary>A single Gantt task line.</summary>
public sealed class GanttTask(string label, string taskId, string start, string durationOrEnd, params string[] tags) : IMermaidable
{
    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var parts = new List<string>(tags.Length + 3);
        parts.AddRange(tags);
        parts.Add(taskId);
        parts.Add(start);
        parts.Add(durationOrEnd);

        var writer = new IndentedStringBuilder();
        writer.WriteLine("{0} :{1}", label, string.Join(", ", parts));
        return writer;
    }
}

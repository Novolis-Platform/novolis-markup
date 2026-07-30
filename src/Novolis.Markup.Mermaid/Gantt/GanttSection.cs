namespace Novolis.Markup.Mermaid;

/// <summary>A named section of Gantt tasks.</summary>
public sealed class GanttSection(string name) : IMermaidable
{
    private readonly List<GanttTask> _tasks = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a task.</summary>
    public GanttSection AddTask(GanttTask task)
    {
        _tasks.Add(task);
        return this;
    }

    /// <summary>Adds a task with common fields.</summary>
    public GanttSection AddTask(string label, string id, string start, string durationOrEnd, params string[] tags)
    {
        _tasks.Add(new GanttTask(label, id, start, durationOrEnd, tags));
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("section {0}", name);
        writer.IncreaseIndent();
        foreach (var task in _tasks)
            writer.WriteLine(task.GetBuilder());
        writer.DecreaseIndent();
        return writer;
    }
}

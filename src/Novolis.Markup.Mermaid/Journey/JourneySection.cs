namespace Novolis.Markup.Mermaid;

/// <summary>A section of user-journey tasks.</summary>
public sealed class JourneySection(string name) : IMermaidable
{
    private readonly List<JourneyTask> _tasks = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a scored task with actors.</summary>
    public JourneySection AddTask(string label, int score, params string[] actors)
    {
        _tasks.Add(new JourneyTask(label, score, actors));
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

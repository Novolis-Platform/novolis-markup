namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>kanban</c> boards.</summary>
public sealed class Kanban : IMermaidable
{
    private readonly List<KanbanColumn> _columns = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a column.</summary>
    public Kanban AddColumn(KanbanColumn column)
    {
        _columns.Add(column);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("kanban");
        writer.IncreaseIndent();
        foreach (var column in _columns)
            writer.WriteLine(column.GetBuilder());
        writer.DecreaseIndent();
        return writer;
    }
}

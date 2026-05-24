namespace Novolis.Markup.Mermaid;

/// <summary>Represents Section.</summary>
public class Section(string title) : IMermaidable
{
    private readonly List<Event> _events = new();
/// <summary>Adds an item.</summary>

    public void AddEvent(Event @event) => _events.Add(@event);
    
    /// <summary>Adds an item.</summary>
    public void AddEvents(IEnumerable<Event> events) => _events.AddRange(events);
    
    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();
    /// <summary>Gets Builder</summary>
    
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("section {0}", title);
        
        writer.IncreaseIndent();
        foreach (var @event in _events)
        {
            writer.WriteLine(@event.GetBuilder());
        }
        
        return writer;
    }
}

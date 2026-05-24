namespace Novolis.Markup.Mermaid;

/// <summary>Represents Timeline.</summary>
public class Timeline(string title) : IMermaidable
{
    private readonly List<Section> _sections = new();
    /// <summary>Adds an item.</summary>
    private readonly List<Event> _events = new();

    /// <summary>Adds an item.</summary>
    public void AddSection(Section section) => _sections.Add(section);
    /// <summary>Adds an item.</summary>
    public void AddSections(IEnumerable<Section> sections) => _sections.AddRange(sections);
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
        writer.WriteLine("timeline");
        writer.IncreaseIndent();
        writer.WriteLine("title {0}", title);

        if (_sections.Any())
        {
            foreach (var section in _sections)
            {
                writer.WriteLine(section.GetBuilder());
            }
            
            writer.DecreaseIndent();
        }
        else
        {
            foreach (var @event in _events)
            {
                writer.WriteLine(@event.GetBuilder());
            }
        }
        
        return writer;
    }
}

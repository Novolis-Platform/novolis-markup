namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>gantt</c> charts.</summary>
public sealed class Gantt(string title) : IMermaidable
{
    private string _dateFormat = "YYYY-MM-DD";
    private readonly List<string> _preamble = [];
    private readonly List<GanttSection> _sections = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Sets the date format token (Mermaid dateFormat).</summary>
    public Gantt WithDateFormat(string format)
    {
        _dateFormat = format;
        return this;
    }

    /// <summary>Sets dates to exclude (weekends, holidays).</summary>
    public Gantt Excludes(string dates)
    {
        _preamble.Add($"excludes {dates}");
        return this;
    }

    /// <summary>Sets the axis tick format.</summary>
    public Gantt AxisFormat(string format)
    {
        _preamble.Add($"axisFormat {format}");
        return this;
    }

    /// <summary>Sets the tick interval (e.g. <c>1week</c>).</summary>
    public Gantt TickInterval(string interval)
    {
        _preamble.Add($"tickInterval {interval}");
        return this;
    }

    /// <summary>Sets the today marker (<c>off</c> or a CSS stroke).</summary>
    public Gantt TodayMarker(string value)
    {
        _preamble.Add($"todayMarker {value}");
        return this;
    }
    /// <summary>Adds a section of tasks.</summary>
    public Gantt AddSection(GanttSection section)
    {
        _sections.Add(section);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("gantt");
        writer.IncreaseIndent();
        writer.WriteLine("title {0}", title);
        writer.WriteLine("dateFormat {0}", _dateFormat);
        foreach (var line in _preamble)
            writer.WriteLine(line);
        foreach (var section in _sections)
            writer.WriteLine(section.GetBuilder());
        writer.DecreaseIndent();
        return writer;
    }
}

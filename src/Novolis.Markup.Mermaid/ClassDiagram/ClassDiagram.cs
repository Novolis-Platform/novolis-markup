namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>classDiagram</c> syntax.</summary>
public sealed class ClassDiagram : IMermaidable
{
    private readonly List<ClassNode> _classes = [];
    private readonly List<ClassRelation> _relations = [];
    private readonly List<string> _notes = [];
    private readonly List<string> _statements = [];

    private Direction? _direction;

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Sets class-diagram direction (<c>direction LR</c>).</summary>
    public ClassDiagram Direction(Direction direction)
    {
        _direction = direction;
        return this;
    }

    /// <summary>Adds a class definition.</summary>
    public ClassDiagram AddClass(ClassNode node)
    {
        _classes.Add(node);
        return this;
    }

    /// <summary>Adds a relationship between classes.</summary>
    public ClassDiagram AddRelation(ClassRelation relation)
    {
        _relations.Add(relation);
        return this;
    }

    /// <summary>Adds a free-form note line.</summary>
    public ClassDiagram AddNote(string note)
    {
        _notes.Add(note);
        return this;
    }

    /// <summary>Appends a raw classDiagram statement.</summary>
    public ClassDiagram AddStatement(string statement)
    {
        _statements.Add(statement);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("classDiagram");
        writer.IncreaseIndent();
        if (_direction is { } dir)
            writer.WriteLine("direction {0}", dir.GetBuilder());
        foreach (var c in _classes)
            writer.WriteLine(c.GetBuilder());
        foreach (var r in _relations)
            writer.WriteLine(r.GetBuilder());
        foreach (var note in _notes)
            writer.WriteLine("note for {0}", note);
        foreach (var statement in _statements)
            writer.WriteLine(statement);
        writer.DecreaseIndent();
        return writer;
    }
}

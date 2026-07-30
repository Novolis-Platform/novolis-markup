namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>classDiagram</c> syntax.</summary>
public sealed class ClassDiagram : IMermaidable
{
    private readonly List<ClassNode> _classes = [];
    private readonly List<ClassRelation> _relations = [];
    private readonly List<string> _notes = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

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

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("classDiagram");
        writer.IncreaseIndent();
        foreach (var c in _classes)
            writer.WriteLine(c.GetBuilder());
        foreach (var r in _relations)
            writer.WriteLine(r.GetBuilder());
        foreach (var note in _notes)
            writer.WriteLine("note for {0}", note);
        writer.DecreaseIndent();
        return writer;
    }
}

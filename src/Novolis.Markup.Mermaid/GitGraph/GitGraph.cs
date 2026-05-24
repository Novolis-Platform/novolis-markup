namespace Novolis.Markup.Mermaid;

/// <summary>Represents GitGraph.</summary>
public class GitGraph : IMermaidable
{
    /// <summary>Commits.</summary>
    public List<Commit> Commits { get; } = new();
    
    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();
    /// <summary>Adds an item.</summary>
    
    public void AddCommit(Commit commit) => Commits.Add(commit);

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("gitGraph");
        writer.IncreaseIndent();
        foreach (var commit in Commits)
        {
            writer.WriteLine(commit.GetBuilder());
        }
        writer.DecreaseIndent();
        return writer;
    }
}

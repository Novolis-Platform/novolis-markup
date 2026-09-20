namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>gitGraph</c> diagrams.</summary>
public class GitGraph : IMermaidable
{
    readonly List<string> _commands = [];
    string _currentBranch = "main";

    /// <summary>Recorded commits (when added via <see cref="AddCommit"/>).</summary>
    public List<Commit> Commits { get; } = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a commit on the current branch, switching branch when needed.</summary>
    public void AddCommit(Commit commit)
    {
        Commits.Add(commit);
        EnsureBranch(commit.Branch);
        _commands.Add(commit.Format());
    }

    /// <summary>Adds a commit on the current branch.</summary>
    public GitGraph Commit(string? id = null, CommitType type = CommitType.Normal, string? tag = null)
    {
        var commit = new Commit(id ?? string.Empty, _currentBranch, type: type, tag: tag);
        if (!string.IsNullOrWhiteSpace(id))
            Commits.Add(commit);
        _commands.Add(commit.Format());
        return this;
    }

    /// <summary>Creates and checks out a branch.</summary>
    public GitGraph Branch(string name)
    {
        _commands.Add($"branch {name}");
        _currentBranch = name;
        return this;
    }

    /// <summary>Checks out an existing branch (<c>checkout</c> / <c>switch</c>).</summary>
    public GitGraph Checkout(string name)
    {
        _commands.Add($"checkout {name}");
        _currentBranch = name;
        return this;
    }

    /// <summary>Merges a branch into the current branch.</summary>
    public GitGraph Merge(string branch)
    {
        _commands.Add($"merge {branch}");
        return this;
    }

    /// <summary>Cherry-picks a commit id onto the current branch.</summary>
    public GitGraph CherryPick(string id)
    {
        _commands.Add($"cherry-pick id:\"{id}\"");
        return this;
    }

    /// <summary>Appends a raw gitGraph command.</summary>
    public GitGraph AddStatement(string statement)
    {
        _commands.Add(statement);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("gitGraph");
        writer.IncreaseIndent();
        foreach (var command in _commands)
            writer.WriteLine(command);
        writer.DecreaseIndent();
        return writer;
    }

    void EnsureBranch(string branch)
    {
        if (string.Equals(branch, _currentBranch, StringComparison.Ordinal))
            return;
        if (string.Equals(_currentBranch, "main", StringComparison.Ordinal)
            && !_commands.Exists(c => c.StartsWith("branch ", StringComparison.Ordinal)))
        {
            Branch(branch);
            return;
        }

        Checkout(branch);
    }
}

namespace Novolis.Markup.Mermaid;

/// <summary>Represents Commit.</summary>
public class Commit : IMermaidable
{
    /// <summary>Commit operation.</summary>
    public Commit(string message, string branch, DateTime? dateTime = null)
    {
        if (dateTime.HasValue)
        {
            Id = new Hash(dateTime.Value);
        }
        Message = message;
        Branch = branch;
    }
    /// <summary>Message.</summary>
    
    public string Message { get; }
    /// <summary>Branch.</summary>
    public string Branch { get; }
    
    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.Write("commit {0}", Id);
        writer.IncreaseIndent();
        writer.WriteLine("message {0}", Message);
        writer.WriteLine("branch {0}", Branch);
        writer.DecreaseIndent();
        return writer;
    }
}

// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents MarkdownHorizontalRule.</summary>
public class MarkdownHorizontalRule : IMarkdownHorizontalRule
{
    /// <summary>ToString operation.</summary>
    public override string ToString() => new string('-', 3);
}

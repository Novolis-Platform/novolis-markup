// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents MarkdownHeader.</summary>
public class MarkdownHeader(string text, int level = 1) : IMarkdownHeader
{
    /// <summary>Text.</summary>
    public string Text { get; } = text;
/// <summary>ToString operation.</summary>

    public int Level { get; } = level;
/// <summary>ToString operation.</summary>

    public override string ToString() => $"{new string('#', Level)} {Text}{new string(IMarkdownHeader.NewLine, 1)}";
}

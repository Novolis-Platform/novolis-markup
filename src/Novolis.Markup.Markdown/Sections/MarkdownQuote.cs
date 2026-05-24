// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents MarkdownQuote.</summary>
public class MarkdownQuote(string text) : IMarkdownQuote
{
    /// <summary>Text.</summary>
    public IEnumerable<string> Text { get; } = text.ReplaceLineEndings(IMarkdownSection.NewLine.ToString()).Split(IMarkdownSection.NewLine);
/// <summary>ToString operation.</summary>

    public override string ToString() => string.Join(IMarkdownSection.NewLine, Text.Select(line => $"> {line}")) + IMarkdownSection.NewLine;
}

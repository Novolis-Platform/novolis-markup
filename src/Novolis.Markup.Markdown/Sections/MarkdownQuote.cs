// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public class MarkdownQuote(string text) : IMarkdownQuote
{
    public IEnumerable<string> Text { get; } = text.ReplaceLineEndings(IMarkdownSection.NewLine.ToString()).Split(IMarkdownSection.NewLine);

    public override string ToString() => string.Join(IMarkdownSection.NewLine, Text.Select(line => $"> {line}")) + IMarkdownSection.NewLine;
}
// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public class MarkdownAlert(string text, MarkdownAlertLevel level) : IMarkdownAlert
{
    public MarkdownAlertLevel Level { get; } = level;

    public IEnumerable<string> Text { get; } = text.TrimStart('>').TrimStart(' ').ReplaceLineEndings(IMarkdownSection.NewLine.ToString()).Split(IMarkdownSection.NewLine);

    public override string ToString() => string.Join(IMarkdownSection.NewLine, Text.Select(line => $"> {line}").Prepend($"> [!{Level.ToString().ToUpper()}]")) + IMarkdownSection.NewLine;
}
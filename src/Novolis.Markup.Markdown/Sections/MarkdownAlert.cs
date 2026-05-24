// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents MarkdownAlert.</summary>
public class MarkdownAlert(string text, MarkdownAlertLevel level) : IMarkdownAlert
{
    /// <summary>Level.</summary>
    public MarkdownAlertLevel Level { get; } = level;
/// <summary>ToString operation.</summary>

    public IEnumerable<string> Text { get; } = text.TrimStart('>').TrimStart(' ').ReplaceLineEndings(IMarkdownSection.NewLine.ToString()).Split(IMarkdownSection.NewLine);
/// <summary>ToString operation.</summary>

    public override string ToString() => string.Join(IMarkdownSection.NewLine, Text.Select(line => $"> {line}").Prepend($"> [!{Level.ToString().ToUpper()}]")) + IMarkdownSection.NewLine;
}

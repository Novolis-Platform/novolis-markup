// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public class MarkdownCodeBlock(string code, string language = "") : IMarkdownCodeBlock
{
    public string Code { get; } = code;

    public string Language { get; } = language;

    public override string ToString() => $"```{Language}{IMarkdownSection.NewLine}{Code}{IMarkdownSection.NewLine}```" + IMarkdownSection.NewLine;
}
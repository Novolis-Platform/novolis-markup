// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents MarkdownCodeBlock.</summary>
public class MarkdownCodeBlock(string code, string language = "") : IMarkdownCodeBlock
{
    /// <summary>Code.</summary>
    public string Code { get; } = code;
/// <summary>ToString operation.</summary>

    public string Language { get; } = language;
/// <summary>ToString operation.</summary>

    public override string ToString() => $"```{Language}{IMarkdownSection.NewLine}{Code}{IMarkdownSection.NewLine}```" + IMarkdownSection.NewLine;
}

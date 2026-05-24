// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents IMarkdownCodeBlock.</summary>
public interface IMarkdownCodeBlock : IMarkdownSection
{
    /// <summary>Code.</summary>
    string Code { get; }
    /// <summary>Language.</summary>
    
    string Language { get; }
}

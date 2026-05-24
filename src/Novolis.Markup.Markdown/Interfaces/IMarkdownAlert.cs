// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents IMarkdownAlert.</summary>
public interface IMarkdownAlert : IMarkdownQuote
{
    /// <summary>Level.</summary>
    MarkdownAlertLevel Level { get; }
}

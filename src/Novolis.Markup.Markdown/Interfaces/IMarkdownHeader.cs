// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents IMarkdownHeader.</summary>
public interface IMarkdownHeader : IMarkdownSection
{
    /// <summary>Text.</summary>
    string Text { get; }
    /// <summary>Level.</summary>
    
    int Level { get; }
}

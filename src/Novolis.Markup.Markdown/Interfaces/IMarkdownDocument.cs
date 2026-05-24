// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents IMarkdownDocument.</summary>
public interface IMarkdownDocument : IEnumerable<IMarkdownSection>
{
    /// <summary>
    /// Compiles the markdown sections into a string, ready for writing to a file or whatever.
    /// </summary>
    /// <returns>
    /// A string containing the markdown document.
    /// </returns>
    string ToString();

    /// <summary>
    /// Adds a section to the Markdown document and returns a new document instance.
    /// </summary>
    /// <param name="section">The section to add to the document.</param>
    /// <returns>A new instance of IMarkdownDocument with the specified section added.</returns>
    IMarkdownDocument With(IMarkdownSection section);

    /// <summary>
    /// Adds the specified sections to the markdown document.
    /// </summary>
    /// <param name="sections">The sections to add.</param>
    /// <returns>The updated markdown document with the added sections.</returns>
    IMarkdownDocument With(IEnumerable<IMarkdownSection> sections);
}

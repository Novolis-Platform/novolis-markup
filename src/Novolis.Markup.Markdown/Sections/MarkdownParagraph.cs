// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>A fluent builder for inline Markdown paragraph content.</summary>
public class MarkdownParagraph : IMarkdownParagraph
{
    private readonly List<IMarkdownParagraphItem> _items = new();

    /// <inheritdoc />
    public IEnumerable<IMarkdownParagraphItem> Items => _items.ToArray();

    /// <summary>Appends bold text.</summary>
    /// <param name="text">The bold span text.</param>
    /// <returns>This paragraph for chaining.</returns>
    public IMarkdownParagraph WithBold(string text)
    {
        _items.Add(new MarkdownParagraphItem(text, MarkdownParagraphItemType.Bold));
        return this;
    }

    /// <summary>Appends italic text.</summary>
    /// <param name="text">The italic span text.</param>
    /// <returns>This paragraph for chaining.</returns>
    public IMarkdownParagraph WithItalic(string text)
    {
        _items.Add(new MarkdownParagraphItem(text, MarkdownParagraphItemType.Italic));
        return this;
    }

    /// <summary>Appends a hyperlink.</summary>
    /// <param name="text">The link label.</param>
    /// <param name="url">The link target URL.</param>
    /// <returns>This paragraph for chaining.</returns>
    public IMarkdownParagraph WithLink(string text, string url)
    {
        _items.Add(new MarkdownParagraphItem(text, MarkdownParagraphItemType.LinkText));
        _items.Add(new MarkdownParagraphItem(url, MarkdownParagraphItemType.Link));
        return this;
    }

    /// <summary>Appends inline code.</summary>
    /// <param name="text">The code span text.</param>
    /// <returns>This paragraph for chaining.</returns>
    public IMarkdownParagraph WithCode(string text)
    {
        _items.Add(new MarkdownParagraphItem(text, MarkdownParagraphItemType.Code));
        return this;
    }

    /// <summary>Appends indentation spans.</summary>
    /// <param name="indent">The number of indent levels to add.</param>
    /// <returns>This paragraph for chaining.</returns>
    public IMarkdownParagraph WithIndent(int indent = 1)
    {
        for (var i = 0; i < indent; i++)
        {
            _items.Add(new MarkdownParagraphItem("    ", MarkdownParagraphItemType.Indent));
        }
        return this;
    }

    /// <summary>Appends a line break within the paragraph.</summary>
    /// <returns>This paragraph for chaining.</returns>
    public IMarkdownParagraph WithNewLine()
    {
        _items.Add(new MarkdownParagraphItem(IMarkdownSection.NewLine.ToString(), MarkdownParagraphItemType.NewLine));
        return this;
    }

    /// <summary>Appends plain text.</summary>
    /// <param name="text">The text span.</param>
    /// <returns>This paragraph for chaining.</returns>
    public IMarkdownParagraph WithText(string text)
    {
        _items.Add(new MarkdownParagraphItem(text, MarkdownParagraphItemType.Text));
        return this;
    }

    /// <inheritdoc />
    public override string ToString() => string.Join(string.Empty, _items.Select(item => item.ToString())) + IMarkdownSection.NewLine;
}

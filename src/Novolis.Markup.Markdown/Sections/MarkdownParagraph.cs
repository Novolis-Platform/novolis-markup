// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public class MarkdownParagraph : IMarkdownParagraph
{
    private readonly List<IMarkdownParagraphItem> _items = new();

    public IEnumerable<IMarkdownParagraphItem> Items => _items.ToArray();
    
    public IMarkdownParagraph WithBold(string text)
    {
        _items.Add(new MarkdownParagraphItem(text, MarkdownParagraphItemType.Bold));
        return this;
    }
    
    public IMarkdownParagraph WithItalic(string text)
    {
        _items.Add(new MarkdownParagraphItem(text, MarkdownParagraphItemType.Italic));
        return this;
    }
    
    public IMarkdownParagraph WithLink(string text, string url)
    {
        _items.Add(new MarkdownParagraphItem(text, MarkdownParagraphItemType.LinkText));
        _items.Add(new MarkdownParagraphItem(url, MarkdownParagraphItemType.Link));
        return this;
    }
    
    public IMarkdownParagraph WithCode(string text)
    {
        _items.Add(new MarkdownParagraphItem(text, MarkdownParagraphItemType.Code));
        return this;
    }
    
    public IMarkdownParagraph WithIndent(int indent = 1)
    {
        for (var i = 0; i < indent; i++)
        {
            _items.Add(new MarkdownParagraphItem("    ", MarkdownParagraphItemType.Indent));
        }
        return this;
    }

    public IMarkdownParagraph WithNewLine()
    {
        _items.Add(new MarkdownParagraphItem(IMarkdownSection.NewLine.ToString(), MarkdownParagraphItemType.NewLine));
        return this;
    }

    public IMarkdownParagraph WithText(string text)
    {
        _items.Add(new MarkdownParagraphItem(text, MarkdownParagraphItemType.Text));
        return this;
    }

    public override string ToString() => string.Join(string.Empty, _items.Select(item => item.ToString())) + IMarkdownSection.NewLine;
}
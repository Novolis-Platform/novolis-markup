using System.Collections;

// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public class MarkdownDocument() : IMarkdownDocument
{
    private readonly SortedList<int, IMarkdownSection> _sections = new();

    public IMarkdownSection this[int index] => _sections[index];
    
    public IEnumerator<IMarkdownSection> GetEnumerator() => _sections.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => string.Join(IMarkdownSection.NewLine, _sections.Values.Select(x => x.ToString())) + IMarkdownSection.NewLine;
    public IMarkdownDocument With(IMarkdownSection section)
    {
        _sections.Add(_sections.Count, section);
        return this;
    }

    public IMarkdownDocument With(IEnumerable<IMarkdownSection> sections)
    {
        foreach (var section in sections)
        {
            _sections.Add(_sections.Count, section);
        }
        return this;
    }
    
    public static IMarkdownDocument Parse(string markdown)
    {
        var document = new MarkdownDocument();
        var groups = markdown.Split($"{IMarkdownSection.NewLine}{IMarkdownSection.NewLine}");
        
        foreach (var group in groups)
        {
            if (group.StartsWith("#"))
            {
                var header = group.Split(IMarkdownSection.NewLine)[0];
                var level = header.TakeWhile(x => x == '#').Count();
                var text = header.Substring(level).Trim();
                document.With(new MarkdownHeader(text, level));
            }
            else if (group.StartsWith(">"))
            {
                var text = group.Split(IMarkdownSection.NewLine).Select(x => x.Substring(1).Trim());
                foreach (string line in text)
                {
                    document.With(new MarkdownQuote(line));
                }
            }
            else if (group.StartsWith("- "))
            {
                var items = group.Split(IMarkdownSection.NewLine).Select(x => x.Substring(2).Trim());
                document.With(new MarkdownUnorderedList(items));
            }
            else if (group.StartsWith("1. "))
            {
                var items = group.Split(IMarkdownSection.NewLine).Select(x => x.Substring(3).Trim());
                document.With(new MarkdownOrderedList(items));
            }
            else if (group.StartsWith("|"))
            {
                var lines = group.Split(IMarkdownSection.NewLine).Select(x => x.Substring(1).Trim()).ToArray();
                var headers = lines.First().Split("|").Select(x => x.Trim()).ToArray();
                var rows = lines.Skip(1).Select(x => x.Split("|").Select(y => y.Trim())).ToArray();
                document.With(new MarkdownTable<string>(headers, rows));
            }
            else
            {
                document.With(new MarkdownParagraph().WithText(group));
            }
        }
        return document;
    }
    
    public static IMarkdownDocument Empty => new MarkdownDocument();
    
    public static IMarkdownDocument Create(params IMarkdownSection[] sections) => new MarkdownDocument().With(sections);
    
    public static IMarkdownDocument Create(IEnumerable<IMarkdownSection> sections) => new MarkdownDocument().With(sections);
    
    public static IMarkdownDocument Create(params string[] sections) => new MarkdownDocument().With(sections.Select(x => new MarkdownParagraph().WithText(x)));
    
    public static IMarkdownDocument Create(IEnumerable<string> sections) => new MarkdownDocument().With(sections.Select(x => new MarkdownParagraph().WithText(x)));
}
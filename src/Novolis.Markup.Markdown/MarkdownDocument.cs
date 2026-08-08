using System.Collections;

// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents MarkdownDocument.</summary>
public class MarkdownDocument() : IMarkdownDocument
{
    private readonly SortedList<int, IMarkdownSection> _sections = new();
/// <summary>Gets Enumerator</summary>

    public IMarkdownSection this[int index] => _sections[index];
    /// <summary>ToString operation.</summary>
    
    public IEnumerator<IMarkdownSection> GetEnumerator() => _sections.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>ToString operation.</summary>
    public override string ToString() => string.Join(IMarkdownSection.NewLine, _sections.Values.Select(x => x.ToString())) + IMarkdownSection.NewLine;
    /// <summary>With operation.</summary>
    public IMarkdownDocument With(IMarkdownSection section)
    {
        _sections.Add(_sections.Count, section);
        return this;
    }
/// <summary>With operation.</summary>

    public IMarkdownDocument With(IEnumerable<IMarkdownSection> sections)
    {
        foreach (var section in sections)
        {
            _sections.Add(_sections.Count, section);
        }
        return this;
    }

    /// <summary>Parses a simple Markdown string into a document.</summary>
    /// <param name="markdown">The Markdown source text.</param>
    /// <returns>A populated document.</returns>
    public static IMarkdownDocument Parse(string markdown)
    {
        var document = new MarkdownDocument();
        var groups = markdown.Replace("\r\n", "\n").Split("\n\n", StringSplitOptions.None);

        foreach (var group in groups)
        {
            if (string.IsNullOrWhiteSpace(group))
                continue;

            var lines = group.Split('\n');
            if (lines[0].StartsWith('#'))
            {
                var header = lines[0];
                var level = header.TakeWhile(static x => x == '#').Count();
                var text = header[level..].Trim();
                document.With(new MarkdownHeader(text, level));
                // Keep Obsidian-style callouts / quotes that share the heading group
                // (no blank line between H1 and `> [!date]` …).
                AppendLeadingQuotes(document, lines.Skip(1));
                var remainder = lines.Skip(1).SkipWhile(static l => l.StartsWith('>')).ToArray();
                if (remainder.Length > 0 && remainder.Any(static l => !string.IsNullOrWhiteSpace(l)))
                    AppendGroup(document, string.Join('\n', remainder));
            }
            else
            {
                AppendGroup(document, group);
            }
        }

        return document;
    }

    static void AppendLeadingQuotes(IMarkdownDocument document, IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            if (!line.StartsWith('>'))
                break;
            document.With(new MarkdownQuote(line[1..].Trim()));
        }
    }

    static void AppendGroup(IMarkdownDocument document, string group)
    {
        if (string.IsNullOrWhiteSpace(group))
            return;

        if (group.StartsWith('>'))
        {
            foreach (var line in group.Split('\n'))
            {
                if (!line.StartsWith('>'))
                    continue;
                document.With(new MarkdownQuote(line[1..].Trim()));
            }
            return;
        }

        if (group.StartsWith("- "))
        {
            var items = group.Split('\n').Where(static l => l.StartsWith("- ")).Select(static x => x[2..].Trim());
            document.With(new MarkdownUnorderedList(items));
            return;
        }

        if (group.StartsWith("1. "))
        {
            var items = group.Split('\n')
                .Where(static l => l.Length > 3 && char.IsDigit(l[0]))
                .Select(static x =>
                {
                    var dot = x.IndexOf(". ", StringComparison.Ordinal);
                    return dot >= 0 ? x[(dot + 2)..].Trim() : x.Trim();
                });
            document.With(new MarkdownOrderedList(items));
            return;
        }

        if (group.StartsWith('|'))
        {
            var lines = group.Split('\n').Select(static x => x.Trim().Trim('|').Trim()).ToArray();
            var headers = lines[0].Split('|').Select(static x => x.Trim()).ToArray();
            var rows = lines.Skip(1)
                .Where(static l => !l.All(static c => c is '-' or '|' or ':' or ' '))
                .Select(static x => x.Split('|').Select(static y => y.Trim()))
                .ToArray();
            document.With(new MarkdownTable<string>(headers, rows));
            return;
        }

        document.With(new MarkdownParagraph().WithText(group));
    }
    /// <summary>Creates a resource.</summary>
    
    public static IMarkdownDocument Empty => new MarkdownDocument();
    
    /// <summary>Creates a resource.</summary>
    public static IMarkdownDocument Create(params IMarkdownSection[] sections) => new MarkdownDocument().With(sections);
    /// <summary>Creates a resource.</summary>
    
    public static IMarkdownDocument Create(IEnumerable<IMarkdownSection> sections) => new MarkdownDocument().With(sections);
    
    /// <summary>Creates a resource.</summary>
    public static IMarkdownDocument Create(params string[] sections) => new MarkdownDocument().With(sections.Select(x => new MarkdownParagraph().WithText(x)));
    
    /// <summary>Creates a resource.</summary>
    public static IMarkdownDocument Create(IEnumerable<string> sections) => new MarkdownDocument().With(sections.Select(x => new MarkdownParagraph().WithText(x)));
}

using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

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

    static readonly Regex OrderedItem = new(@"^(?<indent>\s*)\d+\.\s+", RegexOptions.CultureInvariant | RegexOptions.Compiled);
    static readonly Regex UnorderedItem = new(@"^(?<indent>\s*)([-*])\s+", RegexOptions.CultureInvariant | RegexOptions.Compiled);
    static readonly Regex ThematicBreak = new(@"^\s{0,3}([-*_])\1{2,}\s*$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
    static readonly Regex FenceOpen = new(@"^\s{0,3}```([\w+-]*)\s*$", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    /// <summary>Parses a Markdown string into a document (Novolis subset: headings, paragraphs with inlines, lists, tables, fences, HR, quotes/callouts).</summary>
    public static IMarkdownDocument Parse(string markdown)
    {
        var document = new MarkdownDocument();
        var text = (markdown ?? string.Empty).Replace("\r\n", "\n");
        var lines = text.Split('\n');
        var i = 0;

        while (i < lines.Length)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
            {
                i++;
                continue;
            }

            var fence = FenceOpen.Match(lines[i]);
            if (fence.Success)
            {
                var lang = fence.Groups[1].Value;
                i++;
                var code = new StringBuilder();
                while (i < lines.Length && !lines[i].TrimStart().StartsWith("```", StringComparison.Ordinal))
                {
                    if (code.Length > 0)
                        code.Append('\n');
                    code.Append(lines[i]);
                    i++;
                }

                if (i < lines.Length)
                    i++; // closing fence
                document.With(new MarkdownCodeBlock(code.ToString(), lang));
                continue;
            }

            if (ThematicBreak.IsMatch(lines[i]) && !lines[i].TrimStart().StartsWith("#", StringComparison.Ordinal))
            {
                // Avoid treating "***" scene breaks that are only asterisks as HR when mid-paragraph;
                // standalone thematic lines become horizontal rules (mapper → SceneBreak).
                document.With(new MarkdownHorizontalRule());
                i++;
                continue;
            }

            if (lines[i].StartsWith('#'))
            {
                var header = lines[i];
                var level = header.TakeWhile(static x => x == '#').Count();
                var headerText = header[level..].Trim();
                document.With(new MarkdownHeader(headerText, level));
                i++;
                while (i < lines.Length && lines[i].StartsWith('>'))
                {
                    AppendQuoteLine(document, lines[i]);
                    i++;
                }

                continue;
            }

            if (lines[i].StartsWith('>'))
            {
                while (i < lines.Length && lines[i].StartsWith('>'))
                {
                    AppendQuoteLine(document, lines[i]);
                    i++;
                }

                continue;
            }

            if (UnorderedItem.IsMatch(lines[i]) && !ThematicBreak.IsMatch(lines[i]))
            {
                var items = new List<string>();
                while (i < lines.Length && UnorderedItem.IsMatch(lines[i]) && !ThematicBreak.IsMatch(lines[i]))
                {
                    var m = UnorderedItem.Match(lines[i]);
                    var depth = NestDepth(m.Groups["indent"].Value);
                    var body = lines[i][m.Length..].Trim();
                    items.Add(EncodeNestDepth(depth) + body);
                    i++;
                }

                document.With(new MarkdownUnorderedList(items));
                continue;
            }

            if (OrderedItem.IsMatch(lines[i]))
            {
                var items = new List<string>();
                while (i < lines.Length && OrderedItem.IsMatch(lines[i]))
                {
                    var m = OrderedItem.Match(lines[i]);
                    var depth = NestDepth(m.Groups["indent"].Value);
                    var body = lines[i][m.Length..].Trim();
                    items.Add(EncodeNestDepth(depth) + body);
                    i++;
                }

                document.With(new MarkdownOrderedList(items));
                continue;
            }

            if (lines[i].StartsWith('|'))
            {
                var tableLines = new List<string>();
                while (i < lines.Length && lines[i].StartsWith('|'))
                {
                    tableLines.Add(lines[i].Trim().Trim('|').Trim());
                    i++;
                }

                if (tableLines.Count > 0)
                {
                    var headers = tableLines[0].Split('|').Select(static x => x.Trim()).ToArray();
                    var rows = tableLines.Skip(1)
                        .Where(static l => !l.All(static c => c is '-' or '|' or ':' or ' '))
                        .Select(static x => x.Split('|').Select(static y => y.Trim()))
                        .ToArray();
                    document.With(new MarkdownTable<string>(headers, rows));
                }

                continue;
            }

            // Paragraph: consume until blank line or block start.
            var paraLines = new List<string>();
            while (i < lines.Length
                   && !string.IsNullOrWhiteSpace(lines[i])
                   && !lines[i].StartsWith('#')
                   && !lines[i].StartsWith('>')
                   && !lines[i].StartsWith("|")
                   && !(UnorderedItem.IsMatch(lines[i]) && !ThematicBreak.IsMatch(lines[i]))
                   && !OrderedItem.IsMatch(lines[i])
                   && !FenceOpen.IsMatch(lines[i])
                   && !ThematicBreak.IsMatch(lines[i]))
            {
                paraLines.Add(lines[i]);
                i++;
            }

            if (paraLines.Count > 0)
                document.With(ParseInlineParagraph(string.Join('\n', paraLines)));
        }

        return document;
    }

    static void AppendQuoteLine(IMarkdownDocument document, string line)
    {
        var body = line[1..].Trim();
        document.With(new MarkdownQuote(body));
    }

    /// <summary>Nest depth from leading spaces (2 spaces or 1 tab ≈ one level).</summary>
    internal static int NestDepth(string indent)
    {
        if (string.IsNullOrEmpty(indent))
            return 0;
        var cols = 0;
        foreach (var ch in indent)
            cols += ch == '\t' ? 2 : 1;
        return System.Math.Clamp(cols / 2, 0, 8);
    }

    /// <summary>Stores nest depth as leading U+0001 markers (stripped by list ToString / PDF mapper).</summary>
    internal static string EncodeNestDepth(int depth) =>
        depth <= 0 ? string.Empty : new string('\u0001', depth);

    /// <summary>Reads nest depth encoded by <see cref="EncodeNestDepth"/>.</summary>
    internal static int DecodeNestDepth(string item, out string body)
    {
        var depth = 0;
        while (depth < item.Length && item[depth] == '\u0001')
            depth++;
        body = item[depth..];
        return depth;
    }

    /// <summary>Parses common inline markers into paragraph items.</summary>
    internal static IMarkdownParagraph ParseInlineParagraph(string text)
    {
        var paragraph = new MarkdownParagraph();
        var i = 0;
        var buffer = new StringBuilder();

        void FlushText()
        {
            if (buffer.Length == 0)
                return;
            paragraph.WithText(buffer.ToString());
            buffer.Clear();
        }

        while (i < text.Length)
        {
            if (text[i] == '`' && TryReadDelimited(text, i, "`", out var code, out var afterCode))
            {
                FlushText();
                paragraph.WithCode(code);
                i = afterCode;
                continue;
            }

            if (text.AsSpan(i).StartsWith("**") && TryReadDelimited(text, i, "**", out var bold, out var afterBold))
            {
                FlushText();
                paragraph.WithBold(bold);
                i = afterBold;
                continue;
            }

            if (text[i] == '*' && TryReadDelimited(text, i, "*", out var italic, out var afterItalic)
                && !text.AsSpan(i).StartsWith("**"))
            {
                FlushText();
                paragraph.WithItalic(italic);
                i = afterItalic;
                continue;
            }

            if (text[i] == '[' && TryReadLink(text, i, out var label, out var url, out var afterLink))
            {
                FlushText();
                paragraph.WithLink(label, url);
                i = afterLink;
                continue;
            }

            buffer.Append(text[i]);
            i++;
        }

        FlushText();
        return paragraph;
    }

    static bool TryReadDelimited(string text, int start, string delim, out string inner, out int end)
    {
        inner = string.Empty;
        end = start;
        if (!text.AsSpan(start).StartsWith(delim))
            return false;
        var open = start + delim.Length;
        var close = text.IndexOf(delim, open, StringComparison.Ordinal);
        if (close < 0)
            return false;
        inner = text[open..close];
        end = close + delim.Length;
        return true;
    }

    static bool TryReadLink(string text, int start, out string label, out string url, out int end)
    {
        label = string.Empty;
        url = string.Empty;
        end = start;
        if (text[start] != '[')
            return false;
        var closeLabel = text.IndexOf(']', start + 1);
        if (closeLabel < 0 || closeLabel + 1 >= text.Length || text[closeLabel + 1] != '(')
            return false;
        var closeUrl = text.IndexOf(')', closeLabel + 2);
        if (closeUrl < 0)
            return false;
        label = text[(start + 1)..closeLabel];
        url = text[(closeLabel + 2)..closeUrl];
        end = closeUrl + 1;
        return true;
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

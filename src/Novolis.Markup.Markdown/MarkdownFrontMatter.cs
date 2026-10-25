using System.Text;

namespace Novolis.Markup.Markdown;

/// <summary>
/// Front matter at the start of a Markdown document:
/// <c>---</c>, then <c>Key: value</c> lines, then a closing <c>---</c>.
/// </summary>
public sealed class MarkdownFrontMatter
{
    readonly List<MarkdownFrontMatterField> _fields;

    /// <summary>Empty front matter (no fields).</summary>
    public static MarkdownFrontMatter Empty { get; } = new();

    /// <summary>Creates empty front matter.</summary>
    public MarkdownFrontMatter()
        : this([])
    {
    }

    /// <summary>Creates front matter from fields. Later duplicates replace earlier ones, ignoring key case.</summary>
    /// <param name="fields">Fields in document order.</param>
    public MarkdownFrontMatter(IEnumerable<MarkdownFrontMatterField> fields)
    {
        ArgumentNullException.ThrowIfNull(fields);
        _fields = [];
        foreach (var field in fields)
            AddOrReplace(field);
    }

    /// <summary>True when no fields are set.</summary>
    public bool IsEmpty => _fields.Count == 0;

    /// <summary>Fields in document order.</summary>
    public IReadOnlyList<MarkdownFrontMatterField> Fields => _fields;

    /// <summary><c>Title</c> field, or <see langword="null"/> when missing or blank.</summary>
    public string? Title => Get("Title");

    /// <summary>
    /// <c>Tags</c> when that field is present; otherwise <c>Keywords</c>.
    /// Comma-separated values, flow sequences (<c>[a, b]</c>), and YAML lists all become separate entries.
    /// </summary>
    public IReadOnlyList<string> Tags =>
        TryGet("Tags", out _) ? GetList("Tags") :
        TryGet("Keywords", out _) ? GetList("Keywords") :
        [];

    /// <summary>Returns a field value, or <see langword="null"/> when the key is missing or blank.</summary>
    /// <param name="key">Field name. Matched without regard to case.</param>
    public string? Get(string key) =>
        TryGet(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : null;

    /// <summary>Looks up a field without regard to case.</summary>
    /// <param name="key">Field name.</param>
    /// <param name="value">Stored value, which may be empty when the key is present.</param>
    public bool TryGet(string key, out string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        foreach (var field in _fields)
        {
            if (field.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = field.Value;
                return true;
            }
        }

        value = string.Empty;
        return false;
    }

    /// <summary>
    /// Splits a field into list entries. A blank or missing field yields an empty list.
    /// One scalar with no comma is a single entry.
    /// </summary>
    /// <param name="key">Field name. Matched without regard to case.</param>
    public IReadOnlyList<string> GetList(string key)
    {
        if (!TryGet(key, out var raw) || string.IsNullOrWhiteSpace(raw))
            return [];

        return SplitList(raw);
    }

    /// <summary>Returns a copy with <paramref name="key"/> set, replacing a same-named field.</summary>
    /// <param name="key">Field name.</param>
    /// <param name="value">Field value.</param>
    public MarkdownFrontMatter With(string key, string? value)
    {
        if (!IsKey(key))
            throw new ArgumentException("Front matter keys cannot contain whitespace or reserved YAML characters.", nameof(key));

        var copy = new MarkdownFrontMatter(_fields);
        copy.AddOrReplace(new MarkdownFrontMatterField(key, value));
        return copy;
    }

    /// <summary>Emits a fenced front matter block, or an empty string when <see cref="IsEmpty"/>.</summary>
    public string ToMarkdown()
    {
        if (IsEmpty)
            return string.Empty;

        var builder = new StringBuilder();
        builder.AppendLine("---");
        foreach (var field in _fields)
        {
            builder.Append(field.Key).Append(':');
            if (field.Value.Length > 0)
                builder.Append(' ').Append(FormatValue(field.Value));
            builder.AppendLine();
        }

        builder.AppendLine("---");
        return builder.ToString();
    }

    /// <summary>
    /// Reads a front matter block starting at <paramref name="start"/> when that line is a <c>---</c> fence
    /// and every following line until the closing fence is a field, a list item, a comment, or blank.
    /// A thematic break that is not a field block returns <see langword="false"/>.
    /// </summary>
    /// <param name="lines">Document lines, without trailing newline characters.</param>
    /// <param name="start">Index of the opening fence.</param>
    /// <param name="frontMatter">Parsed fields when this method returns <see langword="true"/>.</param>
    /// <param name="nextIndex">Index of the first line after the closing fence.</param>
    public static bool TryParse(
        IReadOnlyList<string> lines,
        int start,
        out MarkdownFrontMatter frontMatter,
        out int nextIndex)
    {
        ArgumentNullException.ThrowIfNull(lines);
        frontMatter = Empty;
        nextIndex = start;
        if ((uint)start >= (uint)lines.Count || !IsFence(lines[start]))
            return false;

        var fields = new List<MarkdownFrontMatterField>();
        string? listKey = null;
        var listItems = new List<string>();
        var sawCloser = false;
        var index = start + 1;

        void FlushList()
        {
            if (listKey is null)
                return;
            fields.Add(new MarkdownFrontMatterField(listKey, JoinList(listItems)));
            listKey = null;
            listItems.Clear();
        }

        for (; index < lines.Count; index++)
        {
            if (IsFence(lines[index]))
            {
                FlushList();
                sawCloser = true;
                index++;
                break;
            }

            var line = lines[index].Trim();
            if (line.Length == 0 || line[0] == '#')
                continue;

            if (listKey is not null && line.StartsWith("- ", StringComparison.Ordinal))
            {
                listItems.Add(Unquote(line[2..].Trim()));
                continue;
            }

            if (!TryReadField(line, out var key, out var value))
                return false;

            FlushList();
            if (value.Length == 0)
            {
                listKey = key;
                continue;
            }

            fields.Add(new MarkdownFrontMatterField(key, NormalizeFlow(value)));
        }

        if (!sawCloser)
            return false;

        frontMatter = new MarkdownFrontMatter(fields);
        nextIndex = index;
        return true;
    }

    void AddOrReplace(MarkdownFrontMatterField field)
    {
        if (!IsKey(field.Key))
            throw new ArgumentException("Front matter keys cannot contain whitespace or reserved YAML characters.", nameof(field));

        for (var i = 0; i < _fields.Count; i++)
        {
            if (_fields[i].Key.Equals(field.Key, StringComparison.OrdinalIgnoreCase))
            {
                _fields[i] = field;
                return;
            }
        }

        _fields.Add(field);
    }

    static string NormalizeFlow(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.Length >= 2 && trimmed[0] == '[' && trimmed[^1] == ']')
            return JoinList(SplitComma(trimmed[1..^1]));
        return value;
    }

    internal static IReadOnlyList<string> SplitList(string raw)
    {
        var trimmed = raw.Trim();
        if (trimmed.Length == 0)
            return [];

        if (trimmed.Contains('\n', StringComparison.Ordinal))
        {
            var items = new List<string>();
            foreach (var line in trimmed.Split('\n'))
            {
                var item = line.Trim();
                if (item.Length == 0)
                    continue;
                if (item.StartsWith("- ", StringComparison.Ordinal))
                    item = item[2..].Trim();
                item = Unquote(item);
                if (item.Length > 0)
                    items.Add(item);
            }

            return items;
        }

        if (trimmed.Length >= 2 && trimmed[0] == '[' && trimmed[^1] == ']')
            return SplitComma(trimmed[1..^1]);

        return SplitComma(trimmed);
    }

    static string JoinList(IReadOnlyList<string> items)
    {
        if (items.Count == 0)
            return string.Empty;

        var builder = new StringBuilder();
        for (var i = 0; i < items.Count; i++)
        {
            if (i > 0)
                builder.Append(", ");
            builder.Append(QuoteListItem(items[i]));
        }

        return builder.ToString();
    }

    static List<string> SplitComma(string raw)
    {
        var items = new List<string>();
        var current = new StringBuilder();
        var quote = '\0';
        for (var i = 0; i < raw.Length; i++)
        {
            var ch = raw[i];
            if (quote == '\0' && ch is '"' or '\'')
            {
                quote = ch;
                continue;
            }

            if (quote != '\0' && ch == quote)
            {
                quote = '\0';
                continue;
            }

            if (quote == '\0' && ch == ',')
            {
                AddItem(items, current);
                continue;
            }

            current.Append(ch);
        }

        AddItem(items, current);
        return items;
    }

    static void AddItem(List<string> items, StringBuilder current)
    {
        var item = Unquote(current.ToString().Trim());
        if (item.Length > 0)
            items.Add(item);
        current.Clear();
    }

    static string QuoteListItem(string item)
    {
        if (item.Contains('"', StringComparison.Ordinal) || item.Contains(',', StringComparison.Ordinal))
            return "\"" + item.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal) + "\"";
        return item;
    }

    static string FormatValue(string value)
    {
        if (value.Contains('\n', StringComparison.Ordinal) || NeedsQuotes(value))
            return "\"" + value
                .Replace("\\", "\\\\", StringComparison.Ordinal)
                .Replace("\"", "\\\"", StringComparison.Ordinal)
                .Replace("\r", "\\r", StringComparison.Ordinal)
                .Replace("\n", "\\n", StringComparison.Ordinal) + "\"";
        return value;
    }

    static bool NeedsQuotes(string value)
    {
        if (value.Length == 0)
            return false;
        if (char.IsWhiteSpace(value[0]) || char.IsWhiteSpace(value[^1]))
            return true;
        return value.Contains(':')
            || value.Contains('#')
            || value.Contains('"')
            || value[0] is '[' or '{' or '*' or '&' or '!' or '|' or '>' or '%' or '@' or '`' or '\'' or '?';
    }

    static bool TryReadField(string line, out string key, out string value)
    {
        key = string.Empty;
        value = string.Empty;
        var colon = line.IndexOf(':');
        if (colon <= 0)
            return false;

        key = line[..colon].Trim();
        if (!IsKey(key))
            return false;

        value = Unquote(line[(colon + 1)..].Trim());
        return true;
    }

    static bool IsKey(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        var trimmed = key.Trim();
        if (trimmed.Length == 0)
            return false;

        foreach (var ch in trimmed)
        {
            if (char.IsWhiteSpace(ch) || ch is ':' or '#' or '{' or '}' or '[' or ']' or ',' or '&' or '*' or '!' or '|' or '>' or '%' or '@' or '`' or '\'' or '"' or '?')
                return false;
        }

        return trimmed[0] is not ('-' or '+');
    }

    static string Unquote(string value)
    {
        if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
            return Unescape(value[1..^1]);

        if (value.Length >= 2 && value[0] == '\'' && value[^1] == '\'')
            return value[1..^1];

        return value;
    }

    static string Unescape(string value)
    {
        if (!value.Contains('\\', StringComparison.Ordinal))
            return value;

        var builder = new StringBuilder(value.Length);
        for (var i = 0; i < value.Length; i++)
        {
            if (value[i] != '\\' || i + 1 >= value.Length)
            {
                builder.Append(value[i]);
                continue;
            }

            i++;
            builder.Append(value[i] switch
            {
                'n' => '\n',
                'r' => '\r',
                '"' => '"',
                '\\' => '\\',
                _ => value[i],
            });
        }

        return builder.ToString();
    }

    static bool IsFence(string line)
    {
        var indent = 0;
        while (indent < line.Length && line[indent] == ' ')
            indent++;
        if (indent > 3)
            return false;
        return line[indent..].TrimEnd() == "---";
    }
}

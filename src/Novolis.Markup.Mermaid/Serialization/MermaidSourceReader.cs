using System.Diagnostics.CodeAnalysis;

namespace Novolis.Markup.Mermaid;

/// <summary>Splits Mermaid source into optional YAML front matter, header token, and body lines.</summary>
public static class MermaidSourceReader
{
    /// <summary>Reads front matter, the first diagram header, and remaining body lines.</summary>
    public static bool TryRead(
        string source,
        out MermaidFrontMatter frontMatter,
        [NotNullWhen(true)] out string? header,
        out IReadOnlyList<string> bodyLines)
    {
        frontMatter = new MermaidFrontMatter();
        header = null;
        bodyLines = [];

        if (string.IsNullOrWhiteSpace(source))
            return false;

        var lines = source.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        var index = 0;
        while (index < lines.Length && string.IsNullOrWhiteSpace(lines[index]))
            index++;

        if (index < lines.Length && lines[index].Trim() == "---")
        {
            index++;
            var yaml = new List<string>();
            while (index < lines.Length && lines[index].Trim() != "---")
            {
                yaml.Add(lines[index]);
                index++;
            }

            if (index >= lines.Length)
                return false;

            index++;
            frontMatter = ParseFrontMatter(yaml);
        }

        while (index < lines.Length)
        {
            var line = lines[index];
            if (string.IsNullOrWhiteSpace(line) || IsComment(line))
            {
                index++;
                continue;
            }

            header = line.Trim();
            index++;
            break;
        }

        if (header is null)
            return false;

        var body = new List<string>();
        while (index < lines.Length)
        {
            body.Add(lines[index]);
            index++;
        }

        bodyLines = body;
        return true;
    }

    /// <summary>True when the line is a Mermaid <c>%%</c> comment.</summary>
    public static bool IsComment(string line)
    {
        var trimmed = line.TrimStart();
        return trimmed.StartsWith("%%", StringComparison.Ordinal);
    }

    /// <summary>Trims a body line and drops comments / blanks. Returns <see langword="null"/> when the line should be ignored.</summary>
    public static string? Meaningful(string line)
    {
        if (IsComment(line))
            return null;
        var trimmed = line.Trim();
        return trimmed.Length == 0 ? null : trimmed;
    }

    /// <summary>Leading space count (tabs count as four spaces).</summary>
    public static int Indent(string line)
    {
        var n = 0;
        foreach (var c in line)
        {
            if (c == ' ')
                n++;
            else if (c == '\t')
                n += 4;
            else
                break;
        }

        return n;
    }

    static MermaidFrontMatter ParseFrontMatter(IReadOnlyList<string> yaml)
    {
        string? title = null, theme = null, look = null, layout = null;
        var inConfig = false;
        foreach (var raw in yaml)
        {
            var line = raw.TrimEnd();
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#'))
                continue;

            var trimmed = line.Trim();
            if (trimmed.Equals("config:", StringComparison.OrdinalIgnoreCase))
            {
                inConfig = true;
                continue;
            }

            if (!line.StartsWith(' ') && !line.StartsWith('\t') && trimmed.Contains(':') && !trimmed.Equals("config:", StringComparison.OrdinalIgnoreCase))
                inConfig = false;

            if (TryKv(trimmed, "title", out var titleValue))
            {
                title = Unquote(titleValue);
                continue;
            }

            if (!inConfig)
                continue;

            if (TryKv(trimmed, "theme", out var themeValue))
                theme = Unquote(themeValue);
            else if (TryKv(trimmed, "look", out var lookValue))
                look = Unquote(lookValue);
            else if (TryKv(trimmed, "layout", out var layoutValue))
                layout = Unquote(layoutValue);
        }

        return new MermaidFrontMatter
        {
            Title = title,
            Theme = theme,
            Look = look,
            Layout = layout,
        };
    }

    static bool TryKv(string trimmed, string key, out string value)
    {
        value = string.Empty;
        var prefix = key + ":";
        if (!trimmed.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return false;
        value = trimmed[prefix.Length..].Trim();
        return value.Length > 0;
    }

    static string Unquote(string value)
    {
        if (value.Length >= 2 && ((value[0] == '"' && value[^1] == '"') || (value[0] == '\'' && value[^1] == '\'')))
            return value[1..^1];
        return value;
    }
}

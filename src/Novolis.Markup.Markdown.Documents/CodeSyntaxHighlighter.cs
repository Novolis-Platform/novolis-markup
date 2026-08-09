using Novolis.Documents;

namespace Novolis.Markup.Markdown.Documents;

/// <summary>
/// Lightweight lexer for PDF code panels (C#-first; falls back to strings/comments for other languages).
/// Produces <see cref="CodeLine"/> spans — no Avalonia / TextMate dependency.
/// </summary>
public static class CodeSyntaxHighlighter
{
    static readonly DocumentColor DefaultInk = DocumentColor.Parse("#1a1a1a");
    static readonly DocumentColor Keyword = DocumentColor.Parse("#0550ae");
    static readonly DocumentColor TypeName = DocumentColor.Parse("#267f99");
    static readonly DocumentColor StringLit = DocumentColor.Parse("#a31515");
    static readonly DocumentColor Comment = DocumentColor.Parse("#6a737d");
    static readonly DocumentColor NumberLit = DocumentColor.Parse("#098658");
    static readonly DocumentColor Preprocessor = DocumentColor.Parse("#af00db");

    static readonly HashSet<string> CSharpKeywords = new(StringComparer.Ordinal)
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
        "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
        "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
        "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
        "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
        "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed",
        "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw",
        "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using",
        "virtual", "void", "volatile", "while", "record", "var", "when", "where", "yield",
        "async", "await", "required", "file", "scoped", "nint", "nuint", "notnull",
        "init", "get", "set", "add", "remove", "partial", "global", "nameof", "with",
    };

    static readonly HashSet<string> CSharpTypes = new(StringComparer.Ordinal)
    {
        "Console", "String", "StringBuilder", "List", "Dictionary", "Task", "Action", "Func",
        "IEnumerable", "IList", "Span", "ReadOnlySpan", "Memory", "CancellationToken",
        "Exception", "ArgumentNullException", "Guid", "DateTime", "TimeSpan", "Path", "File",
        "Directory", "HttpClient", "JsonSerializer", "Encoding", "Math", "Array",
    };

    /// <summary>Highlights <paramref name="code"/> into styled lines for the given language hint.</summary>
    public static IReadOnlyList<CodeLine> Highlight(string code, string? language)
    {
        var text = (code ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n');
        var rawLines = text.Length == 0 ? [""] : text.Split('\n');
        var lang = NormalizeLanguage(language);
        var result = new List<CodeLine>(rawLines.Length);

        var inBlockComment = false;
        foreach (var raw in rawLines)
        {
            result.Add(new CodeLine { Spans = TokenizeLine(raw, lang, ref inBlockComment) });
        }

        return result;
    }

    static string NormalizeLanguage(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return "csharp";
        var l = language.Trim().ToLowerInvariant();
        return l switch
        {
            "cs" or "c#" or "csharp" => "csharp",
            "fs" or "f#" or "fsharp" => "csharp", // keyword-ish reuse
            "js" or "javascript" or "ts" or "typescript" => "csharp",
            "xml" or "html" or "xaml" => "markup",
            "bash" or "sh" or "shell" or "ps1" or "powershell" => "shell",
            "json" or "yml" or "yaml" => "data",
            _ => l,
        };
    }

    static IReadOnlyList<CodeSpan> TokenizeLine(string line, string lang, ref bool inBlockComment)
    {
        if (lang == "markup")
            return TokenizeMarkup(line);
        if (lang is "shell" or "data")
            return TokenizeShellOrData(line, lang);

        return TokenizeCSharpFamily(line, ref inBlockComment);
    }

    static IReadOnlyList<CodeSpan> TokenizeCSharpFamily(string line, ref bool inBlockComment)
    {
        var spans = new List<CodeSpan>();
        var i = 0;
        while (i < line.Length)
        {
            if (inBlockComment)
            {
                var end = line.IndexOf("*/", i, StringComparison.Ordinal);
                if (end < 0)
                {
                    spans.Add(new CodeSpan(line[i..], Comment));
                    return spans;
                }

                spans.Add(new CodeSpan(line[i..(end + 2)], Comment));
                i = end + 2;
                inBlockComment = false;
                continue;
            }

            if (i + 1 < line.Length && line[i] == '/' && line[i + 1] == '/')
            {
                spans.Add(new CodeSpan(line[i..], Comment));
                return spans;
            }

            if (i + 1 < line.Length && line[i] == '/' && line[i + 1] == '*')
            {
                var end = line.IndexOf("*/", i + 2, StringComparison.Ordinal);
                if (end < 0)
                {
                    spans.Add(new CodeSpan(line[i..], Comment));
                    inBlockComment = true;
                    return spans;
                }

                spans.Add(new CodeSpan(line[i..(end + 2)], Comment));
                i = end + 2;
                continue;
            }

            if (line[i] == '#')
            {
                spans.Add(new CodeSpan(line[i..], Preprocessor));
                return spans;
            }

            if (line[i] is '"' or '\'')
            {
                var quote = line[i];
                var j = i + 1;
                while (j < line.Length)
                {
                    if (line[j] == '\\' && j + 1 < line.Length)
                    {
                        j += 2;
                        continue;
                    }

                    if (line[j] == quote)
                    {
                        j++;
                        break;
                    }

                    j++;
                }

                // verbatim @"..."
                if (quote == '"' && i > 0 && line[i - 1] == '@')
                {
                    // already included @ in previous span; fine
                }

                spans.Add(new CodeSpan(line[i..j], StringLit));
                i = j;
                continue;
            }

            if (line[i] == '@' && i + 1 < line.Length && line[i + 1] == '"')
            {
                var j = i + 2;
                while (j < line.Length)
                {
                    if (line[j] == '"' && j + 1 < line.Length && line[j + 1] == '"')
                    {
                        j += 2;
                        continue;
                    }

                    if (line[j] == '"')
                    {
                        j++;
                        break;
                    }

                    j++;
                }

                spans.Add(new CodeSpan(line[i..j], StringLit));
                i = j;
                continue;
            }

            if (char.IsDigit(line[i]) || (line[i] == '.' && i + 1 < line.Length && char.IsDigit(line[i + 1])))
            {
                var j = i + 1;
                while (j < line.Length && (char.IsLetterOrDigit(line[j]) || line[j] is '.' or '_' or 'x' or 'X'))
                    j++;
                spans.Add(new CodeSpan(line[i..j], NumberLit));
                i = j;
                continue;
            }

            if (IsIdentStart(line[i]))
            {
                var j = i + 1;
                while (j < line.Length && IsIdentPart(line[j]))
                    j++;
                var word = line[i..j];
                DocumentColor? color = null;
                if (CSharpKeywords.Contains(word))
                    color = Keyword;
                else if (CSharpTypes.Contains(word))
                    color = TypeName;
                spans.Add(new CodeSpan(word, color));
                i = j;
                continue;
            }

            // punctuation / whitespace — emit run of non-special
            var k = i + 1;
            while (k < line.Length
                   && !IsIdentStart(line[k])
                   && !char.IsDigit(line[k])
                   && line[k] is not ('"' or '\'' or '/' or '#' or '@')
                   && !(line[k] == '.' && k + 1 < line.Length && char.IsDigit(line[k + 1])))
                k++;
            spans.Add(new CodeSpan(line[i..k], DefaultInk));
            i = k;
        }

        return spans.Count == 0 ? [new CodeSpan(string.Empty)] : spans;
    }

    static IReadOnlyList<CodeSpan> TokenizeMarkup(string line)
    {
        var spans = new List<CodeSpan>();
        var i = 0;
        while (i < line.Length)
        {
            if (line[i] == '<' && i + 3 < line.Length && line.AsSpan(i).StartsWith("<!--"))
            {
                var end = line.IndexOf("-->", i + 4, StringComparison.Ordinal);
                if (end < 0)
                {
                    spans.Add(new CodeSpan(line[i..], Comment));
                    return spans;
                }

                spans.Add(new CodeSpan(line[i..(end + 3)], Comment));
                i = end + 3;
                continue;
            }

            if (line[i] == '<')
            {
                var j = i + 1;
                while (j < line.Length && line[j] != '>')
                    j++;
                if (j < line.Length)
                    j++;
                spans.Add(new CodeSpan(line[i..j], Keyword));
                i = j;
                continue;
            }

            if (line[i] == '"')
            {
                var j = i + 1;
                while (j < line.Length && line[j] != '"')
                    j++;
                if (j < line.Length)
                    j++;
                spans.Add(new CodeSpan(line[i..j], StringLit));
                i = j;
                continue;
            }

            var k = i + 1;
            while (k < line.Length && line[k] is not ('<' or '"'))
                k++;
            spans.Add(new CodeSpan(line[i..k], DefaultInk));
            i = k;
        }

        return spans.Count == 0 ? [new CodeSpan(string.Empty)] : spans;
    }

    static IReadOnlyList<CodeSpan> TokenizeShellOrData(string line, string lang)
    {
        var trimmed = line.TrimStart();
        if (lang == "shell" && (trimmed.StartsWith('#') || trimmed.StartsWith("::")))
            return [new CodeSpan(line, Comment)];

        var spans = new List<CodeSpan>();
        var i = 0;
        while (i < line.Length)
        {
            if (line[i] is '"' or '\'')
            {
                var q = line[i];
                var j = i + 1;
                while (j < line.Length && line[j] != q)
                    j++;
                if (j < line.Length)
                    j++;
                spans.Add(new CodeSpan(line[i..j], StringLit));
                i = j;
                continue;
            }

            if (char.IsDigit(line[i]))
            {
                var j = i + 1;
                while (j < line.Length && (char.IsDigit(line[j]) || line[j] == '.'))
                    j++;
                spans.Add(new CodeSpan(line[i..j], NumberLit));
                i = j;
                continue;
            }

            var k = i + 1;
            while (k < line.Length && line[k] is not ('"' or '\'') && !char.IsDigit(line[k]))
                k++;
            spans.Add(new CodeSpan(line[i..k], DefaultInk));
            i = k;
        }

        return spans.Count == 0 ? [new CodeSpan(string.Empty)] : spans;
    }

    static bool IsIdentStart(char c) => char.IsLetter(c) || c is '_' or '@';

    static bool IsIdentPart(char c) => char.IsLetterOrDigit(c) || c == '_';
}

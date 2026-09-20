using System.Text;

namespace Novolis.Markup.Mermaid;

/// <summary>Optional YAML front matter for a Mermaid document (title, theme, look, layout).</summary>
public sealed class MermaidFrontMatter
{
    /// <summary>Document title (<c>title:</c>).</summary>
    public string? Title { get; init; }

    /// <summary>Theme name (<c>config.theme</c>), e.g. <c>default</c> or <c>forest</c>.</summary>
    public string? Theme { get; init; }

    /// <summary>Look (<c>config.look</c>): <c>classic</c>, <c>neo</c>, or <c>handDrawn</c>.</summary>
    public string? Look { get; init; }

    /// <summary>Layout engine (<c>config.layout</c>): <c>dagre</c> or <c>elk</c>.</summary>
    public string? Layout { get; init; }

    /// <summary>True when no front-matter fields are set.</summary>
    public bool IsEmpty =>
        string.IsNullOrWhiteSpace(Title)
        && string.IsNullOrWhiteSpace(Theme)
        && string.IsNullOrWhiteSpace(Look)
        && string.IsNullOrWhiteSpace(Layout);

    /// <summary>Emits a Mermaid YAML front-matter block, or empty string when <see cref="IsEmpty"/>.</summary>
    public string ToYamlBlock()
    {
        if (IsEmpty)
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("---");
        if (!string.IsNullOrWhiteSpace(Title))
            sb.Append("title: ").AppendLine(QuoteYaml(Title));

        if (!string.IsNullOrWhiteSpace(Theme) || !string.IsNullOrWhiteSpace(Look) || !string.IsNullOrWhiteSpace(Layout))
        {
            sb.AppendLine("config:");
            if (!string.IsNullOrWhiteSpace(Theme))
                sb.Append("  theme: ").AppendLine(Theme);
            if (!string.IsNullOrWhiteSpace(Look))
                sb.Append("  look: ").AppendLine(Look);
            if (!string.IsNullOrWhiteSpace(Layout))
                sb.Append("  layout: ").AppendLine(Layout);
        }

        sb.AppendLine("---");
        return sb.ToString();
    }

    static string QuoteYaml(string value) =>
        value.Contains(':') || value.Contains('#') || value.Contains(' ')
            ? $"\"{value.Replace("\"", "\\\"", StringComparison.Ordinal)}\""
            : value;
}

using System.Text;

namespace Novolis.Markup.Html.Css;

/// <summary>Fluent CSS stylesheet builder.</summary>
public sealed class CssStylesheet : IHtmlNode
{
    private readonly List<string> _blocks = new();

    /// <summary>Adds a rule for <paramref name="selector"/>.</summary>
    public CssStylesheet Rule(string selector, Action<CssRule> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(selector);
        var rule = new CssRule(selector);
        configure(rule);
        _blocks.Add(rule.ToString());
        return this;
    }

    /// <summary>Adds a raw CSS block.</summary>
    public CssStylesheet Raw(string css)
    {
        if (!string.IsNullOrWhiteSpace(css))
        {
            _blocks.Add(css.Trim());
        }

        return this;
    }

    /// <summary>Adds an <c>@media</c> query.</summary>
    public CssStylesheet Media(string query, Action<CssStylesheet> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        var inner = new CssStylesheet();
        configure(inner);
        var sb = new StringBuilder();
        sb.Append("@media ").Append(query).AppendLine(" {");
        foreach (var block in inner._blocks)
        {
            foreach (var line in block.Split('\n'))
            {
                sb.Append("  ").AppendLine(line.TrimEnd('\r'));
            }
        }

        sb.Append('}');
        _blocks.Add(sb.ToString());
        return this;
    }

    /// <summary>Adds an <c>@keyframes</c> block.</summary>
    public CssStylesheet Keyframes(string name, Action<CssKeyframes> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var frames = new CssKeyframes(name);
        configure(frames);
        _blocks.Add(frames.ToString());
        return this;
    }

    /// <summary>Adds <c>:root</c> custom properties.</summary>
    public CssStylesheet Root(Action<CssRule> configure) => Rule(":root", configure);

    /// <inheritdoc />
    public void WriteTo(TextWriter writer) => writer.Write(ToString());

    /// <inheritdoc />
    public override string ToString() => string.Join("\n\n", _blocks);
}

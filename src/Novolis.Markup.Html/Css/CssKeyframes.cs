using System.Text;

namespace Novolis.Markup.Html.Css;

/// <summary>Fluent <c>@keyframes</c> builder.</summary>
public sealed class CssKeyframes(string name)
{
    private readonly List<string> _frames = new();

    /// <summary>Animation name.</summary>
    public string Name { get; } = name;

    /// <summary>Adds a keyframe at <paramref name="offset"/> (e.g. <c>0%</c>, <c>from</c>, <c>to</c>).</summary>
    public CssKeyframes Frame(string offset, Action<CssRule> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(offset);
        var rule = new CssRule(string.Empty);
        configure(rule);
        _frames.Add($"  {offset} {{ {rule.Declarations} }}");
        return this;
    }

    /// <summary>Adds a <c>from</c> frame.</summary>
    public CssKeyframes From(Action<CssRule> configure) => Frame("from", configure);

    /// <summary>Adds a <c>to</c> frame.</summary>
    public CssKeyframes To(Action<CssRule> configure) => Frame("to", configure);

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("@keyframes ").Append(Name).AppendLine(" {");
        foreach (var frame in _frames)
        {
            sb.AppendLine(frame);
        }

        sb.Append('}');
        return sb.ToString();
    }
}

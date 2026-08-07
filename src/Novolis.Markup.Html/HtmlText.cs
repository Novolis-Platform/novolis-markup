namespace Novolis.Markup.Html;

/// <summary>An escaped text node.</summary>
public sealed class HtmlText(string text) : IHtmlNode
{
    /// <summary>The raw (unescaped) text.</summary>
    public string Text { get; } = text ?? string.Empty;

    /// <inheritdoc />
    public void WriteTo(TextWriter writer) => writer.Write(HtmlEscape.Text(Text));

    /// <inheritdoc />
    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}

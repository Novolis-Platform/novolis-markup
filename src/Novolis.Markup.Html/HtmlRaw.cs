namespace Novolis.Markup.Html;

/// <summary>Unescaped markup (stylesheets, scripts, trusted fragments).</summary>
public sealed class HtmlRaw(string markup) : IHtmlNode
{
    /// <summary>The raw markup.</summary>
    public string Markup { get; } = markup ?? string.Empty;

    /// <inheritdoc />
    public void WriteTo(TextWriter writer) => writer.Write(Markup);

    /// <inheritdoc />
    public override string ToString() => Markup;
}

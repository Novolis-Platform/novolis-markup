namespace Novolis.Markup.Html;

/// <summary>Composite factories for lists, tables, code, and alerts.</summary>
public static partial class HtmlMarkup
{
    /// <summary>Creates a <c>ul</c> with text items.</summary>
    public static HtmlElement Ul(IEnumerable<string> items) =>
        Ul(ul =>
        {
            foreach (var item in items)
            {
                ul.Li(item);
            }
        });

    /// <summary>Creates an <c>ol</c> with text items.</summary>
    public static HtmlElement Ol(IEnumerable<string> items) =>
        Ol(ol =>
        {
            foreach (var item in items)
            {
                ol.Li(item);
            }
        });

    /// <summary>Creates a simple table from headers and row cells.</summary>
    public static HtmlElement Table(IEnumerable<string> headers, IEnumerable<IEnumerable<string>> rows) =>
        Table(table =>
        {
            table.Thead(thead => thead.Tr(tr =>
            {
                foreach (var header in headers)
                {
                    tr.Th(header);
                }
            }));
            table.Tbody(tbody =>
            {
                foreach (var row in rows)
                {
                    tbody.Tr(tr =>
                    {
                        foreach (var cell in row)
                        {
                            tr.Td(cell);
                        }
                    });
                }
            });
        });

    /// <summary>Creates <c>pre&gt;code</c> for a fenced code block.</summary>
    public static HtmlElement PreCode(string code, string? language = null) =>
        Pre(pre =>
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                pre.Code(code);
            }
            else
            {
                pre.Code(c => c.Class($"language-{language}").Text(code));
            }
        });

    /// <summary>Creates a Bootstrap-style alert panel.</summary>
    public static HtmlElement Alert(string level, string text) =>
        Div(d => d
            .Class("alert", $"alert-{level}")
            .P(p => p.Strong(level))
            .P(text));

    /// <summary>Creates a <c>blockquote</c> from inline pieces.</summary>
    public static HtmlElement Blockquote(IEnumerable<string> parts) =>
        Blockquote(b =>
        {
            foreach (var part in parts)
            {
                b.Text(part);
            }
        });
}

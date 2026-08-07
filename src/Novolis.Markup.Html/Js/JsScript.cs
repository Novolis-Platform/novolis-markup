using System.Text;

namespace Novolis.Markup.Html.Js;

/// <summary>Fluent JavaScript builder for functions and classes.</summary>
public sealed class JsScript : IHtmlNode
{
    private readonly List<string> _parts = new();

    /// <summary>Adds a function declaration.</summary>
    public JsScript Function(string name, IEnumerable<string> parameters, string body)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var parms = string.Join(", ", parameters);
        _parts.Add($"function {name}({parms}) {{\n{IndentBody(body)}\n}}");
        return this;
    }

    /// <summary>Adds a function with a body builder.</summary>
    public JsScript Function(string name, IEnumerable<string> parameters, Action<JsBody> configure)
    {
        var body = new JsBody();
        configure(body);
        return Function(name, parameters, body.ToString());
    }

    /// <summary>Adds a class declaration.</summary>
    public JsScript Class(string name, Action<JsClass> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var jsClass = new JsClass(name);
        configure(jsClass);
        _parts.Add(jsClass.ToString());
        return this;
    }

    /// <summary>Adds a raw JavaScript statement or block.</summary>
    public JsScript Raw(string javaScript)
    {
        if (!string.IsNullOrWhiteSpace(javaScript))
        {
            _parts.Add(javaScript.Trim());
        }

        return this;
    }

    /// <summary>Adds a <c>const</c> binding.</summary>
    public JsScript Const(string name, string expression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _parts.Add($"const {name} = {expression};");
        return this;
    }

    /// <summary>Adds a <c>let</c> binding.</summary>
    public JsScript Let(string name, string expression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _parts.Add($"let {name} = {expression};");
        return this;
    }

    /// <inheritdoc />
    public void WriteTo(TextWriter writer) => writer.Write(ToString());

    /// <inheritdoc />
    public override string ToString() => string.Join("\n\n", _parts);

    internal static string IndentBody(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return string.Empty;
        }

        var lines = body.Replace("\r\n", "\n", StringComparison.Ordinal).Trim().Split('\n');
        var sb = new StringBuilder();
        for (var i = 0; i < lines.Length; i++)
        {
            if (i > 0)
            {
                sb.Append('\n');
            }

            sb.Append("  ").Append(lines[i]);
        }

        return sb.ToString();
    }
}

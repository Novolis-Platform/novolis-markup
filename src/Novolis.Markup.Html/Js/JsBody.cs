using System.Text;

namespace Novolis.Markup.Html.Js;

/// <summary>Simple statement accumulator for function/method bodies.</summary>
public sealed class JsBody
{
    private readonly List<string> _lines = new();

    /// <summary>Appends a statement (semicolon added if missing).</summary>
    public JsBody Line(string statement)
    {
        if (string.IsNullOrWhiteSpace(statement))
        {
            return this;
        }

        var trimmed = statement.Trim();
        if (!trimmed.EndsWith(';') && !trimmed.EndsWith('}') && !trimmed.EndsWith('{'))
        {
            trimmed += ";";
        }

        _lines.Add(trimmed);
        return this;
    }

    /// <summary>Appends a <c>return</c> statement.</summary>
    public JsBody Return(string expression)
    {
        _lines.Add($"return {expression};");
        return this;
    }

    /// <summary>Appends a <c>const</c> binding.</summary>
    public JsBody Const(string name, string expression)
    {
        _lines.Add($"const {name} = {expression};");
        return this;
    }

    /// <summary>Appends a <c>let</c> binding.</summary>
    public JsBody Let(string name, string expression)
    {
        _lines.Add($"let {name} = {expression};");
        return this;
    }

    /// <summary>Appends an <c>if</c> block.</summary>
    public JsBody If(string condition, Action<JsBody> then)
    {
        var inner = new JsBody();
        then(inner);
        _lines.Add($"if ({condition}) {{");
        foreach (var line in inner._lines)
        {
            _lines.Add("  " + line);
        }

        _lines.Add("}");
        return this;
    }

    /// <summary>Appends raw lines.</summary>
    public JsBody Raw(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            foreach (var line in text.Replace("\r\n", "\n", StringComparison.Ordinal).Trim().Split('\n'))
            {
                _lines.Add(line);
            }
        }

        return this;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < _lines.Count; i++)
        {
            if (i > 0)
            {
                sb.Append('\n');
            }

            sb.Append(_lines[i]);
        }

        return sb.ToString();
    }
}

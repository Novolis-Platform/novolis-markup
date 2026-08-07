using System.Text;

namespace Novolis.Markup.Html.Js;

/// <summary>Fluent JavaScript class builder.</summary>
public sealed class JsClass(string name)
{
    private readonly List<string> _members = new();
    private string? _extends;

    /// <summary>Class name.</summary>
    public string Name { get; } = name;

    /// <summary>Sets <c>extends</c>.</summary>
    public JsClass Extends(string baseClass)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseClass);
        _extends = baseClass;
        return this;
    }

    /// <summary>Adds a constructor.</summary>
    public JsClass Constructor(IEnumerable<string> parameters, string body)
    {
        var parms = string.Join(", ", parameters);
        _members.Add($"  constructor({parms}) {{\n{Indent(body)}\n  }}");
        return this;
    }

    /// <summary>Adds a constructor with a body builder.</summary>
    public JsClass Constructor(IEnumerable<string> parameters, Action<JsBody> configure)
    {
        var body = new JsBody();
        configure(body);
        return Constructor(parameters, body.ToString());
    }

    /// <summary>Adds an instance method.</summary>
    public JsClass Method(string name, IEnumerable<string> parameters, string body)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var parms = string.Join(", ", parameters);
        _members.Add($"  {name}({parms}) {{\n{Indent(body)}\n  }}");
        return this;
    }

    /// <summary>Adds an instance method with a body builder.</summary>
    public JsClass Method(string name, IEnumerable<string> parameters, Action<JsBody> configure)
    {
        var body = new JsBody();
        configure(body);
        return Method(name, parameters, body.ToString());
    }

    /// <summary>Adds an async instance method.</summary>
    public JsClass AsyncMethod(string name, IEnumerable<string> parameters, string body)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var parms = string.Join(", ", parameters);
        _members.Add($"  async {name}({parms}) {{\n{Indent(body)}\n  }}");
        return this;
    }

    /// <summary>Adds a getter.</summary>
    public JsClass Getter(string name, string body)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _members.Add($"  get {name}() {{\n{Indent(body)}\n  }}");
        return this;
    }

    /// <summary>Adds a setter.</summary>
    public JsClass Setter(string name, string parameter, string body)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _members.Add($"  set {name}({parameter}) {{\n{Indent(body)}\n  }}");
        return this;
    }

    /// <summary>Adds a static method.</summary>
    public JsClass StaticMethod(string name, IEnumerable<string> parameters, string body)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        var parms = string.Join(", ", parameters);
        _members.Add($"  static {name}({parms}) {{\n{Indent(body)}\n  }}");
        return this;
    }

    /// <summary>Adds a class field initializer.</summary>
    public JsClass Field(string name, string expression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _members.Add($"  {name} = {expression};");
        return this;
    }

    /// <summary>Adds a static field initializer.</summary>
    public JsClass StaticField(string name, string expression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _members.Add($"  static {name} = {expression};");
        return this;
    }

    /// <summary>Adds a raw class member.</summary>
    public JsClass Raw(string member)
    {
        if (string.IsNullOrWhiteSpace(member))
        {
            return this;
        }

        var trimmed = member.Trim();
        _members.Add(trimmed.StartsWith("  ", StringComparison.Ordinal) ? trimmed : "  " + trimmed);
        return this;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class ").Append(Name);
        if (_extends is not null)
        {
            sb.Append(" extends ").Append(_extends);
        }

        sb.AppendLine(" {");
        for (var i = 0; i < _members.Count; i++)
        {
            if (i > 0)
            {
                sb.AppendLine();
            }

            sb.AppendLine(_members[i]);
        }

        sb.Append('}');
        return sb.ToString();
    }

    private static string Indent(string body)
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

            sb.Append("    ").Append(lines[i]);
        }

        return sb.ToString();
    }
}

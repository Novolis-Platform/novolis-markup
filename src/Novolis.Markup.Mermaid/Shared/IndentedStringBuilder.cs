using System.Text;

namespace Novolis.Markup.Mermaid;

/// <summary>Indent-aware <see cref="StringBuilder"/> for Mermaid diagram text.</summary>
public class IndentedStringBuilder : IIndentedStringBuilder
{
    private StringBuilder _builder = new StringBuilder();
    private int _indentLevel;
    private readonly string _indentString;

    /// <summary>Creates a builder with the given indent string (default four spaces).</summary>
    /// <param name="indentString">Text repeated per indent level.</param>
    public IndentedStringBuilder(string indentString = "    ")
    {
        _indentString = indentString;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder IncreaseIndent()
    {
        _indentLevel++;
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder DecreaseIndent()
    {
        if (_indentLevel > 0)
            _indentLevel--;
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder Write(string text)
    {
        _builder.Append(new string(_indentString[0], _indentLevel * _indentString.Length));
        _builder.Append(text);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder WriteLine(string line = "")
    {
        _builder.Append(new string(_indentString[0], _indentLevel * _indentString.Length));
        _builder.AppendLine(line);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder Write(string format, params object?[] args)
    {
        string formattedText = string.Format(format, args);
        _builder.Append(new string(_indentString[0], _indentLevel * _indentString.Length));
        _builder.Append(formattedText);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder WriteLine(string format, params object[] args)
    {
        string formattedText = string.Format(format, args);
        _builder.Append(new string(_indentString[0], _indentLevel * _indentString.Length));
        _builder.AppendLine(formattedText);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder WriteLine(IIndentedStringBuilder other)
    {
        if (other is IndentedStringBuilder otherBuilder)
        {
            string[] lines = otherBuilder.ToString().Split([Environment.NewLine], StringSplitOptions.None);
            foreach (var line in lines.Where(l => l.Length > 0))
            {
                WriteLine(line);
            }
        }
        return this;
    }

    /// <inheritdoc />
    public override string ToString() => _builder.ToString();
}

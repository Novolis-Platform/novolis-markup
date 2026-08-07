using System.Net;
using System.Text;

namespace Novolis.Markup.Html;

/// <summary>HTML / attribute escaping helpers.</summary>
public static class HtmlEscape
{
    /// <summary>Escapes text for use as an HTML text node.</summary>
    public static string Text(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return WebUtility.HtmlEncode(value);
    }

    /// <summary>Escapes a value for use inside a double-quoted HTML attribute.</summary>
    public static string Attribute(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(value.Length + 8);
        foreach (var ch in value)
        {
            switch (ch)
            {
                case '&':
                    sb.Append("&amp;");
                    break;
                case '"':
                    sb.Append("&quot;");
                    break;
                case '<':
                    sb.Append("&lt;");
                    break;
                case '\0':
                    break;
                default:
                    sb.Append(ch);
                    break;
            }
        }

        return sb.ToString();
    }
}

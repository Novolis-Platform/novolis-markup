using System.Text;
using System.Xml;

namespace Novolis.Markup.Svg;

internal static class SvgSerialization
{
    public static string Serialize(SvgElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        var builder = new StringBuilder();
        using var textWriter = new StringWriter(builder);
        Write(element, textWriter);
        return builder.ToString();
    }

    public static void Write(SvgElement element, TextWriter textWriter)
    {
        ArgumentNullException.ThrowIfNull(element);
        ArgumentNullException.ThrowIfNull(textWriter);
        using var writer = XmlWriter.Create(
            textWriter,
            new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = false,
                NewLineHandling = NewLineHandling.Entitize,
                CloseOutput = false,
            });
        element.WriteTo(writer);
        writer.Flush();
    }
}

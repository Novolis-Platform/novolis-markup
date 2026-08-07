namespace Novolis.Markup.Html;

/// <summary>A node that can be written as HTML/XML markup.</summary>
public interface IHtmlNode
{
    /// <summary>Writes this node to <paramref name="writer"/>.</summary>
    void WriteTo(TextWriter writer);
}

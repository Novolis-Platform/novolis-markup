// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public static class MarkdownTableExtensions
{
    /// <summary>
    /// Convert an IMarkdownTable object to a string in Markdown table format.
    /// </summary>
    /// <param name="table">The IMarkdownTable object to convert.</param>
    /// <returns>A string in Markdown table format representing the given IMarkdownTable object.</returns>
    public static string ToMarkdownTableString(this IMarkdownTable table)
    {
        // Create header
        var header = GenerateHeader(table.Headers);
        
        // Create separator
        var separator = GenerateSeparator(table.Headers);

        // Create lines
        var lines = new List<string> { header, separator };
        lines.AddRange(table.Rows.Select(row => "| " + string.Join(" | ", row) + " |"));

        return string.Join(IMarkdownSection.NewLine, lines) + IMarkdownSection.NewLine;
    }

    private static string GenerateSeparator(IEnumerable<string> headers)
    {
        return "| " + string.Join(" | ", headers.Select(_ => "---")) + " |";
    }

    private static string GenerateHeader(IEnumerable<string> headers)
    {
        return "| " + string.Join(" | ", headers) + " |";
    }
}
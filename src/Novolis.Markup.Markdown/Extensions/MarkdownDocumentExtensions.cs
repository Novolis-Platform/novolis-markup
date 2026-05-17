// ReSharper disable CheckNamespace

namespace Novolis.Markup.Markdown;

/// <summary>
/// Provides extension methods for converting and manipulating Markdown documents.
/// </summary>
public static class MarkdownDocumentExtensions
{
    /// <summary>
    /// Converts the given Markdown document to HTML.
    /// </summary>
    /// <param name="document">The instance of the IMarkdownDocument representing the Markdown document.</param>
    /// <returns>
    /// A string representing the HTML generated from the Markdown document.
    /// </returns>
    public static string ToHtml(this IMarkdownDocument document)
    {
        return MarkdownToHtmlConverter.Convert(document);
    }

    /// <summary>
    /// Adds a paragraph to the markdown document.
    /// </summary>
    /// <param name="builder">The markdown document builder.</param>
    /// <param name="paragraph">The paragraph to add.</param>
    /// <returns>The updated markdown document builder.</returns>
    /// <example>
    /// <code>
    /// IMarkdownDocument document = new MarkdownDocument();
    /// IMarkdownParagraph paragraph = new MarkdownParagraph("This is a paragraph.");
    /// document = document.WithParagraph(paragraph);
    /// </code>
    /// </example>
    public static IMarkdownDocument WithParagraph(this IMarkdownDocument builder, IMarkdownParagraph paragraph)
    {
        return builder.With(paragraph);
    }

    /// <summary>
    /// Adds a horizontal rule to the markdown document.
    /// </summary>
    /// <param name="builder">The markdown document builder.</param>
    /// <param name="horizontalRule">The horizontal rule element.</param>
    /// <returns>The markdown document builder with the added horizontal rule.</returns>
    public static IMarkdownDocument WithHorizontalRule(this IMarkdownDocument builder, IMarkdownHorizontalRule horizontalRule)
    {
        return builder.With(horizontalRule);
    }

    /// <summary>
    /// Adds an alert to the markdown document.
    /// </summary>
    /// <param name="builder">The markdown document builder.</param>
    /// <param name="alert">The alert to be added.</param>
    /// <returns>The updated markdown document.</returns>
    public static IMarkdownDocument WithAlert(this IMarkdownDocument builder, IMarkdownAlert alert)
    {
        return builder.With(alert);
    }

    /// <summary>
    /// Adds an alert to the markdown document.
    /// </summary>
    /// <param name="builder">The markdown document builder.</param>
    /// <param name="message">The message of the alert.</param>
    /// <param name="level">The level of the alert.</param>
    /// <returns>The updated markdown document with the added alert.</returns>
    public static IMarkdownDocument WithAlert(this IMarkdownDocument builder, string message, MarkdownAlertLevel level)
    {
        return builder.WithAlert(new MarkdownAlert(message, level));
    }


    /// <summary>
    /// Appends a code block to the markdown document.
    /// </summary>
    /// <param name="builder">The markdown document builder.</param>
    /// <param name="codeBlock">The code block to append.</param>
    /// <returns>The updated markdown document with the appended code block.</returns>
    public static IMarkdownDocument WithCodeBlock(this IMarkdownDocument builder, IMarkdownCodeBlock codeBlock)
    {
        return builder.With(codeBlock);
    }

    /// <summary>
    /// Adds a code block to the markdown document with the specified code and language.
    /// </summary>
    /// <param name="builder">The markdown document builder.</param>
    /// <param name="code">The code to be included in the code block.</param>
    /// <param name="language">The language of the code (optional).</param>
    /// <returns>The updated markdown document builder.</returns>
    public static IMarkdownDocument WithCodeBlock(this IMarkdownDocument builder, string code, string language = "")
    {
        return builder.WithCodeBlock(new MarkdownCodeBlock(code, language));
    }

    /// <summary>
    /// Adds a header to the Markdown document.
    /// </summary>
    /// <param name="builder">The Markdown document builder.</param>
    /// <param name="header">The header to be added.</param>
    /// <returns>The Markdown document with the added header.</returns>
    public static IMarkdownDocument WithHeader(this IMarkdownDocument builder, IMarkdownHeader header)
    {
        return builder.With(header);
    }

    /// <summary>
    /// Adds a header to the given markdown document.
    /// </summary>
    /// <param name="builder">The markdown document builder.</param>
    /// <param name="text">The text of the header.</param>
    /// <param name="level">The level of the header (default is 1).</param>
    /// <returns>The updated markdown document builder.</returns>
    public static IMarkdownDocument WithHeader(this IMarkdownDocument builder, string text, int level = 1)
    {
        return builder.WithHeader(new MarkdownHeader(text, level));
    }

    /// <summary>
    /// Adds a header with the specified text and level to the markdown document.
    /// </summary>
    /// <param name="builder">The markdown document builder.</param>
    /// <param name="text">The text to be added as the header.</param>
    /// <param name="level">The level of the header (1-6).</param>
    /// <returns>The markdown document builder with the added header.</returns>
    public static IMarkdownDocument WithHeader(this IMarkdownDocument builder, string text, MarkdownHeaderLevel level)
    {
        return builder.WithHeader(new MarkdownHeader(text, level.ToInt()));
    }

    /// <summary>
    /// Appends an ordered list to the markdown document.
    /// </summary>
    /// <param name="builder">The markdown document builder.</param>
    /// <param name="orderedList">The ordered list to be appended.</param>
    /// <returns>The updated markdown document.</returns>
    public static IMarkdownDocument WithOrderedList(this IMarkdownDocument builder, IMarkdownOrderedList orderedList)
    {
        return builder.With(orderedList);
    }

    /// <summary>
    /// Appends an ordered list to the current markdown document.
    /// </summary>
    /// <param name="builder">The markdown document builder.</param>
    /// <param name="items">The items to be added to the ordered list.</param>
    /// <returns>The updated markdown document with the appended ordered list.</returns>
    public static IMarkdownDocument WithOrderedList(this IMarkdownDocument builder, params string[] items)
    {
        return builder.WithOrderedList(new MarkdownOrderedList(items));
    }

    /// <summary>
    /// Adds an ordered list to the Markdown document.
    /// </summary>
    /// <param name="builder">The Markdown document builder.</param>
    /// <param name="items">The items to add to the ordered list.</param>
    /// <param name="style">The style of the ordered list. Default is MarkdownOrderedListStyle.Numbered.</param>
    /// <returns>The updated Markdown document.</returns>
    public static IMarkdownDocument WithOrderedList(this IMarkdownDocument builder, IEnumerable<string> items, MarkdownOrderedListStyle style = MarkdownOrderedListStyle.Numbered)
    {
        return builder.WithOrderedList(new MarkdownOrderedList(items, style));
    }

    /// <summary>
    /// Adds the specified table to the markdown document.
    /// </summary>
    /// <param name="builder">The markdown document builder.</param>
    /// <param name="table">The table to be added.</param>
    /// <returns>
    /// The markdown document with the table added.
    /// </returns>
    public static IMarkdownDocument WithTable(this IMarkdownDocument builder, IMarkdownTable table)
    {
        return builder.With(table);
    }

    /// <summary>
    /// Adds a table to the markdown document using the given items.
    /// </summary>
    /// <typeparam name="T">The type of items in the table.</typeparam>
    /// <param name="builder">The markdown document builder.</param>
    /// <param name="items">The items to include in the table.</param>
    /// <returns>The markdown document with the added table.</returns>
    public static IMarkdownDocument WithTable<T>(this IMarkdownDocument builder, IEnumerable<T> items)
    {
        return builder.WithTable(new MarkdownTable<T>(items));
    }

    /// <summary>
    /// Adds an unordered list to the Markdown document.
    /// </summary>
    /// <param name="builder">The Markdown document builder.</param>
    /// <param name="unorderedList">The unordered list to be added.</param>
    /// <returns>The updated Markdown document.</returns>
    public static IMarkdownDocument WithUnorderedList(this IMarkdownDocument builder, IMarkdownUnorderedList unorderedList)
    {
        return builder.With(unorderedList);
    }

    /// <summary>
    /// Adds an unordered list to the markdown document.
    /// </summary>
    /// <param name="builder">The markdown document builder.</param>
    /// <param name="items">The items to be added to the unordered list.</param>
    /// <returns>The updated markdown document.</returns>
    public static IMarkdownDocument WithUnorderedList(this IMarkdownDocument builder, params string[] items)
    {
        return builder.WithUnorderedList(new MarkdownUnorderedList(items));
    }

    /// <summary>
    /// Adds an unordered list to the markdown document.
    /// </summary>
    /// <param name="builder">The markdown document builder.</param>
    /// <param name="items">The items to be added to the unordered list.</param>
    /// <returns>The updated markdown document.</returns>
    public static IMarkdownDocument WithUnorderedList(this IMarkdownDocument builder, IEnumerable<string> items)
    {
        return builder.WithUnorderedList(new MarkdownUnorderedList(items));
    }
}
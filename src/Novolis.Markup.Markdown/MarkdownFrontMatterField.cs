namespace Novolis.Markup.Markdown;

/// <summary>One field in a Markdown front matter block (<c>Title: …</c>, <c>Tags: …</c>).</summary>
public sealed class MarkdownFrontMatterField
{
    /// <summary>Creates a field. <paramref name="key"/> keeps the caller's spelling.</summary>
    /// <param name="key">Field name.</param>
    /// <param name="value">Field value. Empty when the key is present with no text.</param>
    public MarkdownFrontMatterField(string key, string? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        Key = key.Trim();
        Value = value ?? string.Empty;
    }

    /// <summary>Field name as written.</summary>
    public string Key { get; }

    /// <summary>Field value, without surrounding quotes.</summary>
    public string Value { get; }
}

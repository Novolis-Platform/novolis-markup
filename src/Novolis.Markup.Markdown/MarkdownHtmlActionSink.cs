namespace Novolis.Markup.Markdown;

/// <summary>Collects copy and preview payloads while Markdown is converted to HTML.</summary>
public sealed class MarkdownHtmlActionSink
{
    private readonly List<string> _codeBlocks = [];
    private readonly List<MarkdownPreviewAsset> _previews = [];

    /// <summary>Directory used to embed local Markdown images as <c>data:</c> URIs.</summary>
    public string? SourceDirectory { get; init; }

    /// <summary>Fenced code blocks in document order.</summary>
    public IReadOnlyList<string> CodeBlocks => _codeBlocks;

    /// <summary>Mermaid diagrams and images in document order.</summary>
    public IReadOnlyList<MarkdownPreviewAsset> Previews => _previews;

    /// <summary>Records a code block and returns its copy URI.</summary>
    public string AddCopy(string code)
    {
        var href = MarkdownHtmlActionUris.Copy(_codeBlocks.Count);
        _codeBlocks.Add(code);
        return href;
    }

    /// <summary>Records a preview asset and returns its fullscreen URI.</summary>
    public string AddPreview(string dataUri, string caption)
    {
        var href = MarkdownHtmlActionUris.Preview(_previews.Count);
        _previews.Add(new MarkdownPreviewAsset(dataUri, caption));
        return href;
    }
}

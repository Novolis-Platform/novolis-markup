using Markdig;

namespace Novolis.Markup.Markdown.Rendering;

/// <summary>Shared Markdig pipeline for GFM-style Markdown rendering.</summary>
public static class MarkdownRenderPipeline
{
    /// <summary>Default pipeline with advanced GFM extensions (tables, task lists, etc.).</summary>
    public static MarkdownPipeline Default { get; } = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();
}

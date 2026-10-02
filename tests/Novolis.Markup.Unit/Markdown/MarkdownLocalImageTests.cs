namespace Novolis.Markup.Markdown.Tests;

public sealed class MarkdownLocalImageTests
{
    [Test]
    public async Task TryEmbed_ReadsLocalPng()
    {
        var directory = Path.Combine(Path.GetTempPath(), "novolis-md-image-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "dot.png");
        await File.WriteAllBytesAsync(
            path,
            Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg=="));

        var uri = MarkdownLocalImage.TryEmbed("dot.png", directory);

        await Assert.That(uri).IsNotNull();
        await Assert.That(uri!).StartsWith("data:image/png;base64,");
        Directory.Delete(directory, recursive: true);
    }

    [Test]
    public async Task TryEmbed_RejectsPathEscape()
    {
        var directory = Path.Combine(Path.GetTempPath(), "novolis-md-image-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var uri = MarkdownLocalImage.TryEmbed("..\\Windows\\win.ini", directory);
        await Assert.That(uri).IsNull();
        Directory.Delete(directory, recursive: true);
    }
}

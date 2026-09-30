using Novolis.Markup.Html;
using Novolis.Markup.Html.Css;
using Novolis.Markup.Html.Js;
using Novolis.Markup.Html.Svg;

namespace Novolis.Markup.Unit.Html;

public class CssBuilderTests
{
    [Test]
    public async Task Rule_EmitsCommonDeclarations()
    {
        var css = HtmlMarkup.Css(sheet => sheet
            .Rule(".card", r => r
                .Flex()
                .Gap("1rem")
                .Padding(16)
                .BorderRadius(8)
                .BackgroundColor("#fff")
                .Color("#111")))
            .ToString();

        await Assert.That(css).Contains(".card {");
        await Assert.That(css).Contains("display: flex;");
        await Assert.That(css).Contains("gap: 1rem;");
        await Assert.That(css).Contains("padding: 16px;");
        await Assert.That(css).Contains("border-radius: 8px;");
    }

    [Test]
    public async Task MediaAndKeyframes_EmitBlocks()
    {
        var css = HtmlMarkup.Css(sheet => sheet
            .Media("(max-width: 600px)", m => m.Rule("body", r => r.FontSize("14px")))
            .Keyframes("fade", k => k
                .From(r => r.Opacity(0))
                .To(r => r.Opacity(1))))
            .ToString();

        await Assert.That(css).Contains("@media (max-width: 600px)");
        await Assert.That(css).Contains("@keyframes fade");
        await Assert.That(css).Contains("opacity: 0;");
    }

    [Test]
    public async Task RootVars_Grid_AndRaw()
    {
        var css = HtmlMarkup.Css(sheet => sheet
            .Root(r => r.Var("accent", "#0a7").Var("--gap", "8px"))
            .Rule(".grid", r => r.Grid().GridTemplateColumns("1fr 1fr").Gap("8px"))
            .Raw("/* comment */"))
            .ToString();

        await Assert.That(css).Contains(":root");
        await Assert.That(css).Contains("--accent: #0a7;");
        await Assert.That(css).Contains("display: grid;");
        await Assert.That(css).Contains("/* comment */");
    }

    [Test]
    public async Task EnumDisplay_ToKebab()
    {
        var css = new CssRule(".x").Display(CssDisplay.InlineBlock).Position(CssPosition.Absolute).ToString();

        await Assert.That(css).Contains("display: inline-block;");
        await Assert.That(css).Contains("position: absolute;");
    }
}

using Novolis.Markup.Html;
using Novolis.Markup.Html.Css;
using Novolis.Markup.Html.Js;
using Novolis.Markup.Html.Svg;

namespace Novolis.Markup.Unit.Html;

public class JsBuilderTests
{
    [Test]
    public async Task FunctionAndClass_EmitDeclarations()
    {
        var js = HtmlMarkup.Js(script => script
            .Function("add", ["a", "b"], body => body.Return("a + b"))
            .Class("Counter", c => c
                .Field("count", "0")
                .Constructor(["initial = 0"], "this.count = initial;")
                .Method("inc", [], "this.count += 1; return this.count;")
                .Getter("value", "return this.count;")
                .StaticMethod("create", [], "return new Counter();")))
            .ToString();

        await Assert.That(js).Contains("function add(a, b)");
        await Assert.That(js).Contains("return a + b;");
        await Assert.That(js).Contains("class Counter");
        await Assert.That(js).Contains("constructor(initial = 0)");
        await Assert.That(js).Contains("inc()");
        await Assert.That(js).Contains("get value()");
        await Assert.That(js).Contains("static create()");
    }

    [Test]
    public async Task ConstLet_AndClassExtends()
    {
        var js = HtmlMarkup.Js(script => script
            .Const("PI", "3.14")
            .Let("n", "0")
            .Class("Child", c => c
                .Extends("Parent")
                .Setter("value", "v", "this._v = v;")
                .AsyncMethod("load", [], "return 1;")
                .StaticField("kind", "'x'")
                .Raw("tag = true;")))
            .ToString();

        await Assert.That(js).Contains("const PI = 3.14;");
        await Assert.That(js).Contains("let n = 0;");
        await Assert.That(js).Contains("extends Parent");
        await Assert.That(js).Contains("set value(v)");
        await Assert.That(js).Contains("async load()");
        await Assert.That(js).Contains("static kind = 'x';");
    }

    [Test]
    public async Task JsBody_IfAndLine()
    {
        var body = new JsBody()
            .Const("x", "1")
            .If("x > 0", b => b.Line("console.log(x)"))
            .Return("x");

        var text = body.ToString();
        await Assert.That(text).Contains("const x = 1;");
        await Assert.That(text).Contains("if (x > 0)");
        await Assert.That(text).Contains("console.log(x);");
        await Assert.That(text).Contains("return x;");
    }
}

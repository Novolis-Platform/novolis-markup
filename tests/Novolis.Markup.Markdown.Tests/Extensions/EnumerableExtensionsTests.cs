using JetBrains.Annotations;

namespace Novolis.Markup.Markdown.Tests.Extensions;

[TestSubject(typeof(EnumerableExtensions))]
public class EnumerableExtensionsTests
{
    public class TestTypeSimple
    {
        public int IntNumber { get; set; }

        public string StrValue { get; set; }

        public Guid GuidValue { get; set; }

        public DateTime Date { get; set; }
    }

    public class TestTypeComplex
    {
        public int IntNumber { get; set; }

        public string StrValue { get; set; }

        public TestTypeSimple SubObject { get; set; }
    }

    [Test]
    public async Task TestToMarkdownTableSimple()
    {
        var items = new List<TestTypeSimple> { new TestTypeSimple { IntNumber = 1, StrValue = "test", GuidValue = Guid.NewGuid(), Date = DateTime.Now }, new TestTypeSimple { IntNumber = 2, StrValue = "example", GuidValue = Guid.NewGuid(), Date = DateTime.Now } };

        var markdownTable = items.ToMarkdownTable();

        await Assert.That(markdownTable).IsNotNull();
        await Assert.That(markdownTable).Contains("IntNumber | StrValue | GuidValue | Date");
        await Assert.That(markdownTable).Contains("---");
    }

    [Test]
    public async Task TestToMarkdownTableComplex()
    {
        var items = new List<TestTypeComplex> { new TestTypeComplex { IntNumber = 1, StrValue = "test", SubObject = new TestTypeSimple() }, new TestTypeComplex { IntNumber = 2, StrValue = "example", SubObject = new TestTypeSimple() } };

        var markdownTable = items.ToMarkdownTable();

        await Assert.That(markdownTable).IsNotNull();
        await Assert.That(markdownTable).Contains("IntNumber | StrValue | SubObject");
        await Assert.That(markdownTable).Contains("---");
    }

    [Test]
    public async Task TestToMarkdownTableEmptyList()
    {
        var items = new List<TestTypeSimple>();

        var markdownTable = items.ToMarkdownTable();

        await Assert.That(markdownTable).IsNotNull();
        await Assert.That(markdownTable).Contains("| IntNumber | StrValue | GuidValue | Date |");
        await Assert.That(markdownTable).Contains("| --- | --- | --- | --- |");
    }

    [Test]
    public async Task TestToMarkdownTableNullList()
    {
        List<TestTypeSimple> items = null;

        await Assert.That(() => items.ToMarkdownTable()).Throws<ArgumentNullException>();
    }
}
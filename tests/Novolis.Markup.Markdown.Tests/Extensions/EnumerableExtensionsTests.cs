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
    public void TestToMarkdownTableSimple()
    {
        var items = new List<TestTypeSimple> { new TestTypeSimple { IntNumber = 1, StrValue = "test", GuidValue = Guid.NewGuid(), Date = DateTime.Now }, new TestTypeSimple { IntNumber = 2, StrValue = "example", GuidValue = Guid.NewGuid(), Date = DateTime.Now } };

        var markdownTable = items.ToMarkdownTable();

        Assert.NotNull(markdownTable);
        Assert.Contains("IntNumber | StrValue | GuidValue | Date", markdownTable);
        Assert.Contains("---", markdownTable);
    }

    [Test]
    public void TestToMarkdownTableComplex()
    {
        var items = new List<TestTypeComplex> { new TestTypeComplex { IntNumber = 1, StrValue = "test", SubObject = new TestTypeSimple() }, new TestTypeComplex { IntNumber = 2, StrValue = "example", SubObject = new TestTypeSimple() } };

        var markdownTable = items.ToMarkdownTable();

        Assert.NotNull(markdownTable);
        Assert.Contains("IntNumber | StrValue | SubObject", markdownTable);
        Assert.Contains("---", markdownTable);
    }

    [Test]
    public void TestToMarkdownTableEmptyList()
    {
        var items = new List<TestTypeSimple>();

        var markdownTable = items.ToMarkdownTable();

        Assert.NotNull(markdownTable);
        Assert.Contains("| IntNumber | StrValue | GuidValue | Date |", markdownTable);
        Assert.Contains("| --- | --- | --- | --- |", markdownTable);
    }

    [Test]
    public void TestToMarkdownTableNullList()
    {
        List<TestTypeSimple> items = null;

        Assert.Throws<ArgumentNullException>(() => items.ToMarkdownTable());
    }
}
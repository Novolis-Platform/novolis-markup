using System.Collections;

using JetBrains.Annotations;

using System.Numerics;


namespace Novolis.Markup.Markdown.Tests;

[TestSubject(typeof(MarkdownDocument))]
public class MarkdownDocumentTests
{

    [Test]
    public async Task CreateFromString_Should_Return_MarkdownDocument()
    {
        var doc = MarkdownDocument.Create("# Example", "Examples is my thing.");
        await Assert.That(doc.ToString()).IsEqualTo("# Example\n\nExamples is my thing.\n\n");
    }

    [Test]
    public async Task Append_AddSection_ReturnsSelf()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var section = new MarkdownHeader("Header");
        
        // Act
        var result = builder.With(section);
        
        // Assert
        await Assert.That(result).IsEqualTo(builder);
    }

    [Test]
    public async Task Append_AddSection_AppendInSectionList()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var section1 = new MarkdownHeader("Header");
        var section2 = new MarkdownHeader("Header", 2);

        // Act
        builder.With(section1);
        builder.With(section2);
        
        // Assert
        await Assert.That(builder.ToString()).IsEqualTo("# Header\n\n## Header\n\n");
    }

    [Test]
    public async Task ToString_NoSections_ReturnsEmpty()
    {
        // Arrange
        var builder = new MarkdownDocument();
        
        // Act
        
        // Assert
        await Assert.That(builder.ToString()).IsEqualTo("\n");
    }

    [Test]
    public async Task ToString_SingleSection_ReturnsSectionString()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var section = new MarkdownHeader("Header");
        
        // Act
        builder.With(section);
        
        // Assert
        await Assert.That(builder.ToString()).IsEqualTo("# Header\n\n");
    }

    [Test]
    public async Task ToString_MultipleSections_ReturnsAllSectionStrings()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var section1 = new MarkdownHeader("Header");
        var section2 = new MarkdownHeader("Header", 2);
        
        // Act
        builder.With(section1);
        builder.With(section2);
        
        // Assert
        await Assert.That(builder.ToString()).IsEqualTo("# Header\n\n## Header\n\n");
    }
    
    [Test]
    public async Task WithOrderedList_AddItems_ReturnsSelf()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var items = new[] {"Item 1", "Item 2"};
        var orderedList = new MarkdownOrderedList(items);
        
        // Act
        var result = builder.WithOrderedList(orderedList);
        
        // Assert
        await Assert.That(result).IsEqualTo(builder);
    }
    
    [Test]
    public async Task WithOrderedList_AddItems_AppendInSectionList()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var items = new[] {"Item 1", "Item 2"};
        var orderedList = new MarkdownOrderedList(items);
        
        // Act
        builder.WithOrderedList(orderedList);
        
        // Assert
        await Assert.That(builder.ToString()).IsEqualTo("1. Item 1\n2. Item 2\n\n");
    }
    
    [Test]
    public async Task WithOrderedList_AddItemsWithStyle_AppendInSectionList()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var items = new[] {"Item 1", "Item 2"};
        var orderedList = new MarkdownOrderedList(items, MarkdownOrderedListStyle.Alpha);
        
        // Act
        builder.WithOrderedList(orderedList);
        
        // Assert
        await Assert.That(builder.ToString()).IsEqualTo("a. Item 1\nb. Item 2\n\n");
    }
    
    [Test]
    public async Task WithUnorderedList_AddItems_ReturnsSelf()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var items = new[] {"Item 1", "Item 2"};
        var unorderedList = new MarkdownUnorderedList(items);
        
        // Act
        var result = builder.WithUnorderedList(unorderedList);
        
        // Assert
        await Assert.That(result).IsEqualTo(builder);
    }
    
    [Test]
    public async Task WithUnorderedList_AddItems_AppendInSectionList()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var items = new[] {"Item 1", "Item 2"};
        var unorderedList = new MarkdownUnorderedList(items);
        
        // Act
        builder.WithUnorderedList(unorderedList);
        
        // Assert
        await Assert.That(builder.ToString()).IsEqualTo("- Item 1\n- Item 2\n\n");
    }
    
    [Test]
    public async Task WithCodeBlock_AddCodeBlock_ReturnsSelf()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var codeBlock = new MarkdownCodeBlock("var x = 1;");
        
        // Act
        var result = builder.WithCodeBlock(codeBlock);
        
        // Assert
        await Assert.That(result).IsEqualTo(builder);
    }
    
    [Test]
    public async Task WithCodeBlock_AddCodeBlock_AppendInSectionList()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var codeBlock = new MarkdownCodeBlock("var x = 1;");
        
        // Act
        builder.WithCodeBlock(codeBlock);
        
        // Assert
        await Assert.That(builder.ToString()).IsEqualTo("```\nvar x = 1;\n```\n\n");
    }
    
    [Test]
    public async Task WithCodeBlock_AddCodeBlockWithLanguage_AppendInSectionList()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var codeBlock = new MarkdownCodeBlock("var x = 1;", "csharp");
        
        // Act
        builder.WithCodeBlock(codeBlock);
        
        // Assert
        await Assert.That(builder.ToString()).IsEqualTo("```csharp\nvar x = 1;\n```\n\n");
    }
    
    [Test]
    public async Task WithTable_AddTable_ReturnsSelf()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var table = new MarkdownTable<MarkdownTableTestRow>(new List<MarkdownTableTestRow>
        {
            new() {Name = "Name 1", Value = 1, Vector = new Vector2(1, 2), Version = new Version(1, 2, 3)},
            new() {Name = "Name 2", Value = 2, Vector = new Vector2(3, 4), Version = new Version(4, 5, 6)}
        });
        
        // Act
        var result = builder.WithTable(table);
        
        // Assert
        await Assert.That(result).IsEqualTo(builder);
    }
    
    [Test]
    public async Task WithTable_AddTable_AppendInSectionList()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var table = new MarkdownTable<MarkdownTableTestRow>(new List<MarkdownTableTestRow>
        {
            new() {Name = "Name 1", Value = 1, Vector = new Vector2(1, 2), Version = new Version(1, 2, 3)},
            new() {Name = "Name 2", Value = 2, Vector = new Vector2(3, 4), Version = new Version(4, 5, 6)}
        });
        builder.WithTable(table);
        
        // Act
        var result = builder.ToString();
        
        // Assert
        await Assert.That(StringComparer.InvariantCulture.Equals("| Name | Value | Vector | Version |\n| --- | --- | --- | --- |\n| Name 1 | 1 | <1, 2> | 1.2.3 |\n| Name 2 | 2 | <3, 4> | 4.5.6 |\n\n", result)).IsTrue();
    }
    
    [Test]
    public async Task WithAlert_AddAlert_ReturnsSelf()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var alert = new MarkdownAlert("This is a warning!", MarkdownAlertLevel.Warning);
        
        // Act
        var result = builder.WithAlert(alert);
        
        // Assert
        await Assert.That(result).IsEqualTo(builder);
    }
    
    [Test]
    public async Task WithAlert_AddAlert_AppendInSectionList()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var alert = new MarkdownAlert("Urgent info that needs immediate user attention to avoid problems.", MarkdownAlertLevel.Warning);
        builder.WithAlert(alert);
        
        // Act
        var result = builder.ToString();
        
        // Assert
        await Assert.That(result).IsEqualTo("> [!WARNING]\n> Urgent info that needs immediate user attention to avoid problems.\n\n");
    }
    
    [Test]
    public async Task WithAlert_AddAlertWithMultipleLines_AppendInSectionList()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var alert = new MarkdownAlert("Urgent info that needs immediate user attention to avoid problems.\n\nThis is a second line.", MarkdownAlertLevel.Warning);
        builder.WithAlert(alert);
        
        // Act
        var result = builder.ToString();
        
        // Assert
        await Assert.That(result).IsEqualTo("> [!WARNING]\n> Urgent info that needs immediate user attention to avoid problems.\n> \n> This is a second line.\n\n");
    }
    
    [Test]
    public async Task WithAlert_AddAlertWithMultipleLinesAndLeadingSpaces_AppendInSectionList()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var alert = new MarkdownAlert("Urgent info that needs immediate user attention to avoid problems.\n\n  This is a second line.", MarkdownAlertLevel.Warning);
        builder.WithAlert(alert);
        
        // Act
        var result = builder.ToString();
        
        // Assert
        await Assert.That(result).IsEqualTo("> [!WARNING]\n> Urgent info that needs immediate user attention to avoid problems.\n> \n>   This is a second line.\n\n");
    }
    
    [Test]
    public async Task WithAlert_AddAlertWithMultipleLinesAndLeadingSpacesAndLeadingGreaterThanSigns_AppendInSectionList()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var alert = new MarkdownAlert("> Urgent info that needs immediate user attention to avoid problems.\n\n  This is a second line.", MarkdownAlertLevel.Warning);
        builder.WithAlert(alert);
        
        // Act
        var result = builder.ToString();
        
        // Assert
        await Assert.That(result).IsEqualTo("> [!WARNING]\n> Urgent info that needs immediate user attention to avoid problems.\n> \n>   This is a second line.\n\n");
    }
    
    [Test]
    public async Task WithParagraph_AddParagraph_ReturnsSelf()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var paragraph = new MarkdownParagraph().WithText("This is a paragraph.");
        
        // Act
        var result = builder.WithParagraph(paragraph);
        
        // Assert
        await Assert.That(result).IsEqualTo(builder);
    }
    
    [Test]
    public async Task WithParagraph_AddParagraph_AppendInSectionList()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var paragraph = new MarkdownParagraph().WithText("This is a paragraph.");
        builder.WithParagraph(paragraph);
        
        // Act
        var result = builder.ToString();
        
        // Assert
        await Assert.That(result).IsEqualTo("This is a paragraph.\n\n");
    }
    
    [Test]
    public async Task WithParagraph_AddParagraphUsingParagraphBuilder_ReturnsSelf()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var paragraphBuilder = new MarkdownParagraph();
        paragraphBuilder.WithCode("var x = 1;");
        paragraphBuilder.WithIndent();
        paragraphBuilder.WithItalic("This is italic text.");
        var paragraph = paragraphBuilder;
        
        // Act
        var result = builder.WithParagraph(paragraph);
        
        // Assert
        await Assert.That(result).IsEqualTo(builder);
    }
    
    [Test]
    public async Task WithParagraph_AddParagraphUsingParagraphBuilder_MultipleLiteraryParagraphs()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var paragraphBuilder = new MarkdownParagraph();
        paragraphBuilder.WithText("This is the beginning of the paragraph. It has a very exciting time.");
        paragraphBuilder.WithNewLine();
        paragraphBuilder.WithIndent();
        paragraphBuilder.WithText("'Twas brillig, and the slithy toves' said the Jabberwock. He was very excited to see the toves.");
        var paragraph = paragraphBuilder;
        builder.WithParagraph(paragraph);
        
        // Act
        var result = builder.ToString();
        
        // Assert
        await Assert.That(result).IsEqualTo("This is the beginning of the paragraph. It has a very exciting time.\n    'Twas brillig, and the slithy toves' said the Jabberwock. He was very excited to see the toves.\n\n");
    }
    
    [Test]
    public async Task WithEverything_AddAllSections_ReturnsSelf()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var section1 = new MarkdownHeader("Header");
        var section2 = new MarkdownHeader("Header", 2);
        var items = new[] {"Item 1", "Item 2"};
        var orderedList = new MarkdownOrderedList(items);
        var unorderedList = new MarkdownUnorderedList(items);
        var codeBlock = new MarkdownCodeBlock("var x = 1;");
        var table = new MarkdownTable<MarkdownTableTestRow>(new List<MarkdownTableTestRow>
        {
            new() {Name = "Name 1", Value = 1, Vector = new Vector2(1, 2), Version = new Version(1, 2, 3)},
            new() {Name = "Name 2", Value = 2, Vector = new Vector2(3, 4), Version = new Version(4, 5, 6)}
        });
        var alert = new MarkdownAlert("Urgent info that needs immediate user attention to avoid problems.", MarkdownAlertLevel.Warning);

        // Act
        var result = builder
            .With(section1)
            .WithHeader(section2)
            .WithOrderedList(orderedList)
            .WithUnorderedList(unorderedList)
            .WithCodeBlock(codeBlock)
            .WithTable(table)
            .WithAlert(alert);
        
        // Assert
        await Assert.That(result).IsEqualTo(builder);
    }
    
    [Test]
    public async Task WithEverything_AddAllSections_AppendInSectionList()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var section1 = new MarkdownHeader("Header");
        var section2 = new MarkdownHeader("Header", 2);
        var items = new[] {"Item 1", "Item 2"};
        var orderedList = new MarkdownOrderedList(items);
        var unorderedList = new MarkdownUnorderedList(items);
        var codeBlock = new MarkdownCodeBlock("var x = 1;");
        var table = new MarkdownTable<MarkdownTableTestRow>(new List<MarkdownTableTestRow>
        {
            new() {Name = "Name 1", Value = 1, Vector = new Vector2(1, 2), Version = new Version(1, 2, 3)},
            new() {Name = "Name 2", Value = 2, Vector = new Vector2(3, 4), Version = new Version(4, 5, 6)}
        });
        var alert = new MarkdownAlert("Urgent info that needs immediate user attention to avoid problems.", MarkdownAlertLevel.Warning);
        builder
            .With(section1)
            .WithHeader(section2)
            .WithOrderedList(orderedList)
            .WithUnorderedList(unorderedList)
            .WithCodeBlock(codeBlock)
            .WithTable(table)
            .WithAlert(alert);
        
        // Act
        var result = builder.ToString();
        
        // Assert
        await Assert.That(result).IsEqualTo("# Header\n\n## Header\n\n1. Item 1\n2. Item 2\n\n- Item 1\n- Item 2\n\n```\nvar x = 1;\n```\n\n| Name | Value | Vector | Version |\n| --- | --- | --- | --- |\n| Name 1 | 1 | <1, 2> | 1.2.3 |\n| Name 2 | 2 | <3, 4> | 4.5.6 |\n\n> [!WARNING]\n> Urgent info that needs immediate user attention to avoid problems.\n\n");
    }
    
    [Test]
    public async Task WithEverything_AddAllSectionsUsingParagraphBuilder_ReturnsSelf()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var section1 = new MarkdownHeader("Header");
        var section2 = new MarkdownHeader("Header", 2);
        var paragraphBuilder = new MarkdownParagraph();
        paragraphBuilder.WithCode("var x = 1;");
        paragraphBuilder.WithIndent();
        paragraphBuilder.WithItalic("This is italic text.");
        var paragraph = paragraphBuilder;
        var items = new[] {"Item 1", "Item 2"};
        var orderedList = new MarkdownOrderedList(items);
        var unorderedList = new MarkdownUnorderedList(items);
        var codeBlock = new MarkdownCodeBlock("var x = 1;");
        var table = new MarkdownTable<MarkdownTableTestRow>(new List<MarkdownTableTestRow>
        {
            new() {Name = "Name 1", Value = 1, Vector = new Vector2(1, 2), Version = new Version(1, 2, 3)},
            new() {Name = "Name 2", Value = 2, Vector = new Vector2(3, 4), Version = new Version(4, 5, 6)}
        });
        var alert = new MarkdownAlert("Urgent info that needs immediate user attention to avoid problems.", MarkdownAlertLevel.Warning);

        // Act
        var result = builder
            .With(section1)
            .WithHeader(section2)
            .WithParagraph(paragraph)
            .WithOrderedList(orderedList)
            .WithUnorderedList(unorderedList)
            .WithCodeBlock(codeBlock)
            .WithTable(table)
            .WithAlert(alert);
        
        // Assert
        await Assert.That(result).IsEqualTo(builder);
    }
    
    [Test]
    public async Task WithEverything_AddAllSectionsUsingParagraphBuilder_MultipleLiteraryParagraphs()
    {
        // Arrange
        var builder = new MarkdownDocument();
        var section1 = new MarkdownHeader("Header");
        var section2 = new MarkdownHeader("Header", 2);
        var paragraphBuilder = new MarkdownParagraph();
        paragraphBuilder.WithText("This is the beginning of the paragraph. It has a very exciting time.");
        paragraphBuilder.WithNewLine();
        paragraphBuilder.WithIndent();
        paragraphBuilder.WithText("'Twas brillig, and the slithy toves' said the Jabberwock. He was very excited to see the toves.");
        var paragraph = paragraphBuilder;
        var items = new[] {"Item 1", "Item 2"};
        var orderedList = new MarkdownOrderedList(items);
        var unorderedList = new MarkdownUnorderedList(items);
        var codeBlock = new MarkdownCodeBlock("var x = 1;");
        var table = new MarkdownTable<MarkdownTableTestRow>(new List<MarkdownTableTestRow>
        {
            new() {Name = "Name 1", Value = 1, Vector = new Vector2(1, 2), Version = new Version(1, 2, 3)},
            new() {Name = "Name 2", Value = 2, Vector = new Vector2(3, 4), Version = new Version(4, 5, 6)}
        });
        var alert = new MarkdownAlert("Urgent info that needs immediate user attention to avoid problems.", MarkdownAlertLevel.Warning);
        builder
            .With(section1)
            .WithHeader(section2)
            .WithParagraph(paragraph)
            .WithOrderedList(orderedList)
            .WithUnorderedList(unorderedList)
            .WithCodeBlock(codeBlock)
            .WithTable(table)
            .WithAlert(alert);
        
        // Act
        var result = builder.ToString();
        
        // Assert
        await Assert.That(result).IsEqualTo("# Header\n\n## Header\n\nThis is the beginning of the paragraph. It has a very exciting time.\n    'Twas brillig, and the slithy toves' said the Jabberwock. He was very excited to see the toves.\n\n1. Item 1\n2. Item 2\n\n- Item 1\n- Item 2\n\n```\nvar x = 1;\n```\n\n| Name | Value | Vector | Version |\n| --- | --- | --- | --- |\n| Name 1 | 1 | <1, 2> | 1.2.3 |\n| Name 2 | 2 | <3, 4> | 4.5.6 |\n\n> [!WARNING]\n> Urgent info that needs immediate user attention to avoid problems.\n\n");
    }

    private class MarkdownTableTestRow
    {
        public string Name { get; set; }
        
        public decimal Value { get; set; }
        
        public Vector2 Vector { get; set; }
        
        public Version Version { get; set; }
    }

    [Test]
    public async Task Constructor_Should_Initialize_Sections_Correctly()
    {
        IMarkdownSection[] sections = { new MarkdownSectionExample(), new MarkdownSectionExample() };
        var doc = new MarkdownDocument();
        foreach (var section in sections)
        {
            doc.With(section);
        }
        await Assert.That(doc.ToString()).IsEqualTo("Example\nExample\n");
    }

    [Test]
    public async Task Markdown_Should_Return_Markdown_String()
    {
        IMarkdownSection[] sections = { new MarkdownSectionExample(), new MarkdownSectionExample() };
        var doc = new MarkdownDocument();
        foreach (var section in sections)
        {
            doc.With(section);
        }
        await Assert.That(doc.ToString()).IsEqualTo("Example\nExample\n");
    }

    [Test]
    public async Task ToString_Should_Return_Markdown_String()
    {
        IMarkdownSection[] sections = { new MarkdownSectionExample(), new MarkdownSectionExample() };
        var doc = new MarkdownDocument();
        doc.With(sections);
        await Assert.That(doc.ToString()).IsEqualTo("Example\nExample\n");
    }

    [Test]
    public async Task GetEnumerator_Should_Return_Enumerator_Over_Sections()
    {
        IMarkdownSection[] sections = { new MarkdownSectionExample(), new MarkdownSectionExample() };
        var doc = new MarkdownDocument()
            .With(sections[0])
            .With(sections[1]);
        IEnumerator enumerator = ((IEnumerable)doc).GetEnumerator();
        using var unknown = enumerator as IDisposable;
        await Assert.That(enumerator.MoveNext()).IsTrue();
        await Assert.That(enumerator.Current).IsSameReferenceAs(sections[0]);
        await Assert.That(enumerator.MoveNext()).IsTrue();
        await Assert.That(enumerator.Current).IsSameReferenceAs(sections[1]);
        await Assert.That(enumerator.MoveNext()).IsFalse();
    }
}

class MarkdownSectionExample : IMarkdownSection
{
    public override string ToString() => "Example";
}
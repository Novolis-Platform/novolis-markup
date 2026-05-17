using System.Text;

namespace Novolis.Markup.Mermaid.Tests;

public class HashTests
{
    [Test]
    public async Task GenerateHash_WithZeroTicksDateTime_ReturnsValidBase36Hash()
    {
        DateTime dateTime = new DateTime(0L, DateTimeKind.Utc);
        var hash = new Hash(dateTime);
        await Assert.That(hash.ToString()).IsNotNullOrWhiteSpace();
    }

    [Test]
    public async Task GenerateHash_WithCurrentDateTime_ReturnsValidBase36Hash()
    {
        DateTime dateTime = DateTime.UtcNow;
        var hash = new Hash(dateTime);
        await Assert.That(hash.ToString()).IsNotNullOrWhiteSpace();
    }

    [Test]
    public async Task GenerateHash_WithSpecificDateTime_ReturnsValidBase36Hash()
    {
        DateTime dateTime = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var hash = new Hash(dateTime);
        await Assert.That(hash.ToString()).IsNotNullOrWhiteSpace();
    }

    [Test]
    public async Task GenerateHash_WithMaxDateTime_ReturnsValidBase36Hash()
    {
        var hash = new Hash(DateTime.MaxValue);
        await Assert.That(hash.ToString()).IsNotNullOrWhiteSpace();
    }

    [Test]
    public async Task GenerateHash_WithMinDateTime_ReturnsValidBase36Hash()
    {
        var hash = new Hash(DateTime.MinValue);
        await Assert.That(hash.ToString()).IsNotNullOrWhiteSpace();
    }

    [Test]
    public async Task GenerateHash_WithNullDateTime_ReturnsValidBase36Hash()
    {
        DateTime? dateTime = null;
        var hash = new Hash(dateTime);
        await Assert.That(hash.ToString()).IsNotNullOrWhiteSpace();
    }

    [Test]
    public async Task GenerateMultipleHashes_SortsByDateTime()
    {
        var hashes = new List<Hash>
        {
            new Hash(DateTime.MinValue),
            new Hash(DateTime.MaxValue),
            new Hash(),
            new Hash(DateTime.Today),
            Hash.Parse("000000000069"),
            Hash.Parse(Hash.Parse("000000000666").ToInt64()),
            new Hash(new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
            new Hash(new DateTime(2022, 1, 2, 0, 0, 0, DateTimeKind.Utc)),
            new Hash(new DateTime(2022, 1, 3, 0, 0, 0, DateTimeKind.Utc)),
            new Hash(new DateTime(2022, 1, 4, 0, 0, 0, DateTimeKind.Utc)),
            new Hash(new DateTime(2022, 1, 5, 0, 0, 0, DateTimeKind.Utc)),
            new Hash(new DateTime(2022, 1, 6, 0, 0, 0, DateTimeKind.Utc)),
            new Hash(new DateTime(2022, 1, 7, 0, 0, 0, DateTimeKind.Utc)),
            new Hash(new DateTime(2022, 1, 8, 0, 0, 0, DateTimeKind.Utc)),
            new Hash(new DateTime(2022, 1, 9, 0, 0, 0, DateTimeKind.Utc)),
            new Hash(new DateTime(2022, 1, 10, 0, 0, 0, DateTimeKind.Utc)),
        }.Order().ToList();

        var result = hashes.Select(x => x.ToString()).Order().ToList();

        for (var index = 0; index < result.Count; index++)
        {
            var hashResult = result[index];
            var hashSource = hashes[index].ToString();
            await Assert.That(hashResult).IsEqualTo(hashSource);
        }

        await Assert.That(result.Count).IsEqualTo(hashes.Count);
    }
}

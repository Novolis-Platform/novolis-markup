using FluentAssertions;

namespace Novolis.Markup.Mermaid.Tests;

// ReSharper disable once CheckNamespace

internal static class AssertShim
{
    public static void Equal<T>(T expected, T actual) => actual.Should().Be(expected);
}

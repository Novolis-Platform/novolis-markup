using FluentAssertions;

namespace Novolis.Markup.Markdown.Tests;

internal static class AssertShim
{
    public static void Equal<T>(T expected, T actual) => actual.Should().Be(expected);

    public static void Equal(string expected, string actual, StringComparer comparer) =>
        comparer.Equals(expected, actual).Should().BeTrue();

    public static void NotNull(object value) => value.Should().NotBeNull();

    public static void Contains(string expected, string actual) => actual.Should().Contain(expected);

    public static void True(bool condition) => condition.Should().BeTrue();

    public static void False(bool condition) => condition.Should().BeFalse();

    public static T Throws<T>(Action action) where T : Exception
    {
        action.Should().Throw<T>();
        return default;
    }
}

namespace Novolis.Markup.Mermaid;

/// <summary>Represents DirectionExtensions.</summary>
public static class DirectionExtensions
{
    /// <summary>Gets Builder</summary>
    public static string GetBuilder(this Direction direction)
    {
        return direction switch
        {
            Direction.TopToBottom => "TB",
            Direction.TopDown => "TD",
            Direction.BottomToTop => "BT",
            Direction.RightToLeft => "RL",
            Direction.LeftToRight => "LR",
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}

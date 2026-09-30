namespace Novolis.Markup.Mermaid;

/// <summary>Event-modeling entity kinds (compact and relaxed tokens).</summary>
public enum EventModelingEntityType
{
    /// <summary>UI (<c>ui</c>).</summary>
    Ui,

    /// <summary>Processor (<c>pcr</c> / <c>processor</c>).</summary>
    Processor,

    /// <summary>Command (<c>cmd</c> / <c>command</c>).</summary>
    Command,

    /// <summary>Read model (<c>rmo</c> / <c>readmodel</c>).</summary>
    ReadModel,

    /// <summary>Event (<c>evt</c> / <c>event</c>).</summary>
    Event,
}

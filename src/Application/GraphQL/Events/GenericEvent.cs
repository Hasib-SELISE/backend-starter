namespace Application.GraphQL.Events;

/// <summary>
/// Generic Event
/// </summary>
public class GenericEvent
{
    public string EventType { get; set; } = string.Empty;
    public dynamic Payload { get; set; } = null!;
    public string JsonPayload { get; set; } = string.Empty;
}
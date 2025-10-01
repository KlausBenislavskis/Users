namespace Users.Api.Events;

/// <summary>
/// Published to message bus when a new user is created.
/// External services can subscribe to this event.
/// </summary>
public record UserCreatedEvent
{
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }

    // For message bus headers
    public string EventType => nameof(UserCreatedEvent);
    public string EventVersion => "v1";
}

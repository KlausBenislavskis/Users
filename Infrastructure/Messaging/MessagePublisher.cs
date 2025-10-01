using System.Text.Json;
using Microsoft.Extensions.Logging;
using Users.Api.Events;
using Users.Application.Common.Interfaces;
using Users.Domain.Users;

namespace Users.Infrastructure.Messaging;

public class MessagePublisher : IMessagePublisher
{
    private readonly ILogger<MessagePublisher> _logger;

    public MessagePublisher(ILogger<MessagePublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishUserCreatedAsync(User user)
    {
        // Create event from Api project (can be consumed by external services)
        var @event = new UserCreatedEvent
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email.Value,
            CreatedAt = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(@event, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        _logger.LogInformation(
            "Publishing {EventType} message to queue: {Message}",
            @event.EventType,
            json);

        // In real implementation:
        // await _rabbitMqClient.PublishAsync("user.events", @event);
        // OR
        // await _messageBus.PublishAsync("user.created", @event);

        return Task.CompletedTask;
    }
}

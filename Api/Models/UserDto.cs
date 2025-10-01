namespace Users.Api.Models;

public record UserDto
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public ProfileDto Profile { get; init; } = null!;
}

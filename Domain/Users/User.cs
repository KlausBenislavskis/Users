using Users.Domain.Common;

namespace Users.Domain.Users;

public class User : Entity
{
    public string Username { get; private set; } = string.Empty;
    public UserEmail Email { get; private set; } = null!;
    public Profile Profile { get; private set; } = null!;

    // EF Core
    private User() { }

    private User(Guid id, string username, UserEmail email, Profile profile) : base(id)
    {
        Username = username;
        Email = email;
        Profile = profile;
    }

    public static User Create(string username, UserEmail email, Profile profile)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        if (username.Length < 3)
            throw new ArgumentException("Username must be at least 3 characters", nameof(username));

        var id = Guid.NewGuid();
        var user = new User(id, username, email, profile);

        // Set the UserId on the profile
        profile.GetType().GetProperty(nameof(Profile.UserId))!.SetValue(profile, id);

        return user;
    }
}

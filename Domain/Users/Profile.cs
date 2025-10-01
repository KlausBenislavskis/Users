using Users.Domain.Common;

namespace Users.Domain.Users;

public class Profile : Entity
{
    public Guid UserId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateTime DateOfBirth { get; private set; }

    // EF Core
    private Profile() { }

    private Profile(Guid id, string firstName, string lastName, DateTime dateOfBirth) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }

    public static Profile Create(string firstName, string lastName, DateTime dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (dateOfBirth >= DateTime.UtcNow)
            throw new ArgumentException("Date of birth must be in the past", nameof(dateOfBirth));

        return new Profile(Guid.NewGuid(), firstName, lastName, dateOfBirth);
    }
}

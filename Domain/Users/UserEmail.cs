using System.Text.RegularExpressions;
using Users.Domain.Common;

namespace Users.Domain.Users;

public partial class UserEmail : ValueObject
{
    public string Value { get; }

    private UserEmail(string value)
    {
        Value = value;
    }

    public static UserEmail Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        return new UserEmail(email);
    }

    private static bool IsValidEmail(string email)
    {
        return EmailRegex().IsMatch(email);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}

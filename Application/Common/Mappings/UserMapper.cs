using Riok.Mapperly.Abstractions;
using Users.Api.Models;
using Users.Domain.Users;

namespace Users.Application.Common.Mappings;

[Mapper]
public partial class UserMapper
{
    // Mapperly generates this at compile time
    public partial UserDto MapToDto(User user);

    // Custom mapping for nested Profile
    private ProfileDto MapProfile(Profile profile) => new()
    {
        FirstName = profile.FirstName,
        LastName = profile.LastName,
        DateOfBirth = profile.DateOfBirth
    };

    // Custom mapping for value object
    private string MapEmail(UserEmail email) => email.Value;
}

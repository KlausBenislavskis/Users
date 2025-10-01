using Users.Api.Models;
using Users.Application.Common;
using Users.Application.Common.Mappings;
using Users.Domain.Users;

namespace Users.Application.Users.Queries.GetUserById;

public static class GetUserByIdQueryHandler
{
    // Wolverine convention: static Handle method
    // Mapperly mapper injected as parameter
    public static async Task<Result<UserDto>> Handle(
        GetUserByIdQuery query,
        IUserRepository repository,
        UserMapper mapper,
        CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(query.Id, cancellationToken);

        if (user is null)
            return Result<UserDto>.Failure("User not found");

        // Use Mapperly's compile-time generated mapping (zero reflection!)
        var dto = mapper.MapToDto(user);
        return Result<UserDto>.Success(dto);
    }
}

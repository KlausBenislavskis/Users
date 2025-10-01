using Users.Application.Common;
using Users.Application.Common.Interfaces;
using Users.Domain.Users;

namespace Users.Application.Users.Commands.CreateUser;

public static class CreateUserCommandHandler
{
    // Wolverine discovers this by convention (static Handle method)
    // Dependencies are injected as parameters
    public static async Task<Result<Guid>> Handle(
        CreateUserCommand command,
        IUserRepository repository,
        IMessagePublisher messagePublisher,
        CancellationToken cancellationToken)
    {
        // Check username uniqueness
        var existingUser = await repository.GetByUsernameAsync(
            command.Username, cancellationToken);

        if (existingUser is not null)
            return Result<Guid>.Failure("Username already exists");

        try
        {
            // Create domain entities
            var email = UserEmail.Create(command.Email);
            var profile = Profile.Create(
                command.FirstName,
                command.LastName,
                command.DateOfBirth);

            var user = User.Create(command.Username, email, profile);

            // Persist
            await repository.AddAsync(user, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            // Publish message (integration event)
            await messagePublisher.PublishUserCreatedAsync(user);

            return Result<Guid>.Success(user.Id);
        }
        catch (ArgumentException ex)
        {
            return Result<Guid>.Failure(ex.Message);
        }
    }
}

namespace Users.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    DateTime DateOfBirth
);

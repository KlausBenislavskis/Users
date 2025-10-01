using Users.Domain.Users;

namespace Users.Application.Common.Interfaces;

public interface IMessagePublisher
{
    Task PublishUserCreatedAsync(User user);
}

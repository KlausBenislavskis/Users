using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Common.Interfaces;
using Users.Domain.Users;
using Users.Infrastructure.Messaging;
using Users.Infrastructure.Persistence;
using Users.Infrastructure.Persistence.Repositories;

namespace Users.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Messaging (uses UserCreatedEvent from Api project)
        services.AddScoped<IMessagePublisher, MessagePublisher>();

        return services;
    }
}

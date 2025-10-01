using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Users.Api.Server.Middleware;
using Users.Application.Common.Mappings;
using Users.Application.Users.Commands.CreateUser;
using Users.Infrastructure;
using Users.Infrastructure.Persistence;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);

// Wolverine configuration (replaces MediatR)
builder.Host.UseWolverine(opts =>
{
    // Auto-discover message handlers from Application assembly
    opts.Discovery.IncludeAssembly(typeof(CreateUserCommand).Assembly);

    // Optional: Configure policies (retries, circuit breakers, etc.)
    opts.Policies.AutoApplyTransactions();
});

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();

// Register Mapperly mapper (singleton - it's stateless and generated)
builder.Services.AddSingleton<UserMapper>();

// Infrastructure services (EF Core, repositories, message publisher)
builder.Services.AddInfrastructure(builder.Configuration);

// API services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Users API", Version = "v1" });
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Apply migrations on startup (not for production!)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();

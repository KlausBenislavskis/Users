using Microsoft.AspNetCore.Mvc;
using Users.Api.Models;
using Users.Application.Users.Commands.CreateUser;
using Users.Application.Users.Queries.GetUserById;
using Wolverine;

namespace Users.Api.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMessageBus messageBus, ILogger<UsersController> logger)
    {
        _messageBus = messageBus;
        _logger = logger;
    }

    /// <summary>
    /// Get user by ID with their profile information
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Getting user with ID: {UserId}", id);

        var result = await _messageBus.InvokeAsync<Application.Common.Result<UserDto>>(
            new GetUserByIdQuery(id), ct);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully retrieved user {Username}", result.Value!.Username);
        }
        else
        {
            _logger.LogWarning("User not found: {UserId}", id);
        }

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { error = result.Error });
    }

    /// <summary>
    /// Create a new user and their profile
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Creating new user: {Username}", request.Username);

        var command = new CreateUserCommand(
            request.Username,
            request.Email,
            request.FirstName,
            request.LastName,
            request.DateOfBirth);

        var result = await _messageBus.InvokeAsync<Application.Common.Result<Guid>>(command, ct);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Successfully created user {Username} with ID: {UserId}",
                request.Username, result.Value);
        }
        else
        {
            _logger.LogWarning("Failed to create user {Username}: {Error}",
                request.Username, result.Error);
        }

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, new CreateUserResponse(result.Value))
            : BadRequest(new { error = result.Error });
    }
}

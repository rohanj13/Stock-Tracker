namespace Portfolio.Application.DTOs;

/// <summary>
/// Represents a user for API responses.
/// </summary>
public sealed class UserDto
{
    public Guid Id { get; set; }

    public string ExternalAuthUserId { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>
/// Request payload for creating a new user.
/// </summary>
public sealed class CreateUserRequest
{
    public string ExternalAuthUserId { get; set; } = string.Empty;
}

/// <summary>
/// Request payload for updating an existing user.
/// </summary>
public sealed class UpdateUserRequest
{
    public string ExternalAuthUserId { get; set; } = string.Empty;
}

using Portfolio.Application.DTOs;
using Portfolio.Application.Interfaces;

namespace Portfolio.Api.Endpoints;

/// <summary>
/// CRUD endpoints for managing users.
/// </summary>
public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users")
            .WithTags("Users");

        group.MapGet("/", GetAllUsers)
            .WithName("GetAllUsers")
            .Produces<IReadOnlyCollection<UserDto>>(StatusCodes.Status200OK)
            .WithSummary("Get all users");

        group.MapGet("/{id:guid}", GetUserById)
            .WithName("GetUserById")
            .Produces<UserDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get a user by id");

        group.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .Accepts<CreateUserRequest>("application/json")
            .Produces<UserDto>(StatusCodes.Status201Created)
            .WithSummary("Create a new user");

        group.MapPut("/{id:guid}", UpdateUser)
            .WithName("UpdateUser")
            .Accepts<UpdateUserRequest>("application/json")
            .Produces<UserDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Update an existing user");

        group.MapDelete("/{id:guid}", DeleteUser)
            .WithName("DeleteUser")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Delete a user");
    }

    private static async Task<IResult> GetAllUsers(IUserService userService, CancellationToken cancellationToken)
    {
        var users = await userService.GetAllAsync(cancellationToken);
        return Results.Ok(users);
    }

    private static async Task<IResult> GetUserById(Guid id, IUserService userService, CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(id, cancellationToken);
        return user is not null ? Results.Ok(user) : Results.NotFound();
    }

    private static async Task<IResult> CreateUser(CreateUserRequest request, IUserService userService, CancellationToken cancellationToken)
    {
        var user = await userService.CreateAsync(request, cancellationToken);
        return Results.Created($"/api/users/{user.Id}", user);
    }

    private static async Task<IResult> UpdateUser(Guid id, UpdateUserRequest request, IUserService userService, CancellationToken cancellationToken)
    {
        var user = await userService.UpdateAsync(id, request, cancellationToken);
        return user is not null ? Results.Ok(user) : Results.NotFound();
    }

    private static async Task<IResult> DeleteUser(Guid id, IUserService userService, CancellationToken cancellationToken)
    {
        var deleted = await userService.DeleteAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}

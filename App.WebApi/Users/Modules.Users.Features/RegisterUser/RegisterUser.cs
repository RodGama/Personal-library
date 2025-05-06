using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.Features;
using Modules.Users.Features.Shared.Responses;
using Modules.Users.Infrastructure.Database;
using Modules.Users.Infrastructure.Database.Repositories;
using Modules.Users.Infrastructure.Database.Repositories.Interfaces;

namespace Modules.Users.Features.RegisterUser
{
    public sealed record RegisterUserRequest(
    string Email,
    string Name,
    string Password,
    DateOnly BirthDate);
    public class RegisterUserEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/users/register", Handle)
                .WithName("RegisterUser")
                .WithTags("Users")
                .Produces<UserResponse>(StatusCodes.Status200OK)
                .Produces<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized); ;
        }

        private static async Task<IResult> Handle(
       [FromBody] RegisterUserRequest request,
       IValidator<RegisterUserRequest> validator,
       IMediator mediator,
       CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var command = request.MapToCommand();

            var response = await mediator.Send(command, cancellationToken);
            if (response.IsError)
            {
                return response.Errors.ToProblem();
            }

            return Results.Ok(response.Value);
        }
    }
    internal sealed record RegisterUserCommand(
    string Email,
    string Name,
    string Password,
    DateOnly BirthDate)
    : IRequest<ErrorOr<UserResponse>>;

    internal sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    ILogger<RegisterUserCommandHandler> logger)
    : IRequestHandler<RegisterUserCommand, ErrorOr<UserResponse>>
    {
        public async Task<ErrorOr<UserResponse>> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            var userExists = await userRepository.GetUserByEmail(request.Email);
            if (userExists is not null)
            {
                logger.LogInformation("User '{Email}' already exists", request.Email);
                return Error.Conflict("Duplicated", "User already exists");
            }

            var user = request.MapToUser();

            await userRepository.Add(user);

            logger.LogInformation("Created user: {@User}", user);

            return user.MapToResponse();
        }
    }

}

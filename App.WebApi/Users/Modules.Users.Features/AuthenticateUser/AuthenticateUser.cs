using Azure;
using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Modules.Common.Features;
using Modules.Users.Features.RegisterUser;
using Modules.Users.Infrastructure.Database.Repositories.Interfaces;

namespace Modules.Users.Features.AuthenticateUser
{
    public sealed record AuthenticateUserRequest(
    string Email,
    string Password);

    public class AuthenticateUserEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/users/login", Handle)
                .WithName("AuthenticateUser")
                .WithTags("Users")
                .Produces<AuthResponse>(StatusCodes.Status200OK)
                .Produces<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);
        }

        private static async Task<IResult> Handle(
       [FromBody] AuthenticateUserRequest request,
       IMediator mediator,
       CancellationToken cancellationToken)
        {
            var command = request.MapToCommand();

            var response = await mediator.Send(command, cancellationToken);
            if (response.IsError)
            {
                return response.Errors.ToProblem();
            }

            return Results.Ok(response.Value);
        }
    }

    internal sealed record AuthenticateUserCommand(
        string Email,
        string Password
        ) : IRequest<ErrorOr<AuthResponse>>;

    internal sealed class AuthenticateUserHandler(
    IUserRepository userRepository,
    ILogger<RegisterUserCommandHandler> logger)
    : IRequestHandler<AuthenticateUserCommand, ErrorOr<AuthResponse>>
    {
        public async Task<ErrorOr<AuthResponse>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByEmail(request.Email);
            if(user is not null)
            {
                if (BCryptor.InputIsCorrect(request.Password,user.Password)
                    && user.Email == request.Email)
                {
                    logger.LogInformation("User '{Email}' authenticated", request.Email);
                    var auth = TokenService.GenerateToken(user);
                    var authResponse = new AuthResponse(
                        auth,
                        true);
                    return authResponse.MapToResponse();
                }
                else
                {
                    logger.LogInformation("Request to login failed - '{Email}'", request.Email);
                    return Error.Unauthorized("Unauthorized", "Password incorrect");
                }
            }
            return Error.Unauthorized("Unauthorized","User not found");

        }
    }
}


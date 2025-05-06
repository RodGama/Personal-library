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
using Modules.Users.Infrastructure.Database.Repositories.Interfaces;

namespace Modules.Users.Features.UpdateUser
{
    public sealed record UpdateUserRequest(
    string Password,
    string TokenReset);
    public class UpdateUserEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/users/update", Handle)
                .WithName("UpdateUser")
                .WithTags("Users")
                .Produces<UserResponse>(StatusCodes.Status200OK)
                .Produces<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
                .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized); ;
        }

        private static async Task<IResult> Handle(
       [FromBody] UpdateUserRequest request,
       HttpContext httpContext,
       IValidator<UpdateUserRequest> validator,
       IMediator mediator,
       CancellationToken cancellationToken)
        {
            var teste = TokenService.DecryptToken(request.TokenReset);
            Guid UserId = Guid.Parse(teste.Token.Replace("Token: ",""));

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var command = request.MapToCommand(UserId);

            var response = await mediator.Send(command, cancellationToken);
            if (response.IsError)
            {
                return response.Errors.ToProblem();
            }

            return Results.Ok(response.Value);
        }
    }
    internal sealed record UpdateUserCommand(
    string Password,
    Guid UserId)
    : IRequest<ErrorOr<UserResponse>>;

    internal sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    ILogger<UpdateUserCommandHandler> logger)
    : IRequestHandler<UpdateUserCommand, ErrorOr<UserResponse>>
    {
        public async Task<ErrorOr<UserResponse>> Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await userRepository.GetById(request.UserId);
            if (user is not null)
            {
                logger.LogInformation("User '{UserId}' already exists", request.UserId);
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                await userRepository.Update(user);
                logger.LogInformation("Updated user: {@User}", user);
            }
            else
            {
                logger.LogInformation("User '{UserId}' not found", request.UserId);
                return Error.NotFound("NotFound", "User not found");
            }

            return user.MapToResponse();
        }
    }

}

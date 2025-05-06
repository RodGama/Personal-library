using Azure.Core;
using Carter;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Modules.Books.Domain;
using Modules.Books.Features.RetriveUserBooks;
using Modules.Books.Features.Shared.Responses;
using Modules.Books.Infrastructure.Database.Repositories.Interfaces;
using Modules.Books.Infrastructure.Migrations;
using Modules.Common.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Books.Features.RetrieveAllUserBooks
{
    public sealed record RetrieveAllUserBooksRequest();
    public class RetrieveAllUserBooksEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/books/all", Handle)
                .WithName("RetrieveAllUserBooks")
                .WithTags("Books")
                .RequireAuthorization()
                .Produces<List<BookResponse>>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        }
        private static async Task<IResult> Handle(
        HttpContext httpContext,
        IMediator mediator,
        CancellationToken cancellationToken)
        {
            Guid UserId = Guid.Parse(httpContext.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value);

            var response = await mediator.Send(new RetrieveAllUserBooksCommand(UserId), cancellationToken);
            if (response.IsError)
            {
                return response.Errors.ToProblem();
            }

            return Results.Ok(response.Value);
        }
    }
    internal sealed record RetrieveAllUserBooksCommand(
        Guid UserId
        ) : IRequest<ErrorOr<List<BookResponse>>>;

    internal sealed class RetrieveAllUserBooksHandler(
        IBookRepository bookRepository,
        ILogger<RetrieveAllUserBooksHandler> logger)
        : IRequestHandler<RetrieveAllUserBooksCommand, ErrorOr<List<BookResponse>>>
    {
        public async Task<ErrorOr<List<BookResponse>>> Handle(RetrieveAllUserBooksCommand command, CancellationToken cancellationToken)
        {
            var books = await bookRepository.GetAllBooksFromUserByIdAsync(command.UserId, cancellationToken);
            if (books == null || !books.Any())
            {
                logger.LogInformation("No books found for user '{UserId}'", command.UserId);
                return Error.NotFound("No books found","User has no books registered");
            }

            logger.LogInformation("Books found for user: {@UserId}", command.UserId);
            return books.ToList().MapToResponse();
        }
    }
}

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

namespace Modules.Books.Features.RetrieveUserBooks
{
    public sealed record RetrieveUserBooksRequest();
    public class RetrieveUserBooksEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/books/retrieve", Handle)
                .WithName("RetrieveUserBooks")
                .WithTags("Books")
                .RequireAuthorization()
                .Produces<List<BookResponse>>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        }
        private static async Task<IResult> Handle(
        [FromQuery] int pageNumber,
        [FromQuery] int pageQuantity,
        HttpContext httpContext,
        IMediator mediator,
        CancellationToken cancellationToken)
        {
            Guid UserId = Guid.Parse(httpContext.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value);

            var response = await mediator.Send(new RetrieveUserBooksCommand(UserId, pageNumber, pageQuantity), cancellationToken);
            if (response.IsError)
            {
                return response.Errors.ToProblem();
            }

            return Results.Ok(response.Value);
        }
    }
    internal sealed record RetrieveUserBooksCommand(
        Guid UserId,
        int PageNumber,
        int PageQuantity
        ) : IRequest<ErrorOr<List<BookResponse>>>;

    internal sealed class RetrieveUserBooksHandler(
        IBookRepository bookRepository,
        ILogger<RetrieveUserBooksHandler> logger)
        : IRequestHandler<RetrieveUserBooksCommand, ErrorOr<List<BookResponse>>>
    {
        public async Task<ErrorOr<List<BookResponse>>> Handle(RetrieveUserBooksCommand command, CancellationToken cancellationToken)
        {
            var books = await bookRepository.GetBooksByUserIdPagedAsync(command.UserId,command.PageNumber,command.PageQuantity, cancellationToken);
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

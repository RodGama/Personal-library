using Azure.Core;
using Carter;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Modules.Books.Features.RetrieveAllUserBooks;
using Modules.Books.Features.Shared.Responses;
using Modules.Books.Infrastructure.Database.Repositories.Interfaces;
using Modules.Common.Features;


namespace Modules.Books.Features.SearchUserBooks
{
    public sealed record SearchUserBooksRequest();

    public class SearchUserBooksEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/books/search", Handle)
                .WithName("SearchUserBooks")
                .WithTags("Books")
                .RequireAuthorization()
                .Produces<List<BookResponse>>(StatusCodes.Status200OK)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        }
        private static async Task<IResult> Handle(
        [FromQuery] string Query,
        [FromQuery] int PageNumber,
        [FromQuery] int PageQuantity,
        HttpContext httpContext,
        IMediator mediator,
        CancellationToken cancellationToken)
        {
            Guid UserId = Guid.Parse(httpContext.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value);

            var response = await mediator.Send(new SearchUserBooksCommand(UserId, Query, PageNumber, PageQuantity), cancellationToken);
            if (response.IsError)
            {
                return response.Errors.ToProblem();
            }

            return Results.Ok(response.Value);
        }
    }
    internal sealed record SearchUserBooksCommand(
        Guid UserId,
        string Query,
        int PageNumber,
        int PageQuantity) : IRequest<ErrorOr<List<BookResponse>>>;

    internal sealed class SearchUserBooksHandler(
        IBookRepository bookRepository,
        ILogger<SearchUserBooksHandler> logger)
        : IRequestHandler<SearchUserBooksCommand, ErrorOr<List<BookResponse>>>
    {
        public async Task<ErrorOr<List<BookResponse>>> Handle(SearchUserBooksCommand command, CancellationToken cancellationToken)
        {
            var books = await bookRepository.SearchUserBooksAsync(command.UserId, command.Query, command.PageNumber,command.PageQuantity, cancellationToken);
            if (books == null || !books.Any())
            {
                logger.LogInformation("No books found for user '{UserId}' already exists", command.UserId);
                return Error.NotFound("No books found", "User has no books registered");
            }

            logger.LogInformation("Books found for user: {@UserId}", command.UserId);
            return books.ToList().MapToResponse();
        }
    }
}

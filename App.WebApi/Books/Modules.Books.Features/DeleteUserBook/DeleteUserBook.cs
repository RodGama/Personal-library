using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Modules.Books.Features.Shared.Responses;
using Modules.Books.Infrastructure.Database.Repositories.Interfaces;
using Modules.Common.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Books.Features.DeleteUserBook
{
    public sealed record DeleteUserBookRequest();

    public class DeleteUserBookEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/books/delete", Handle)
                .WithName("DeleteUserBook")
                .WithTags("Books")
                .RequireAuthorization()
                .Produces<BookResponse>(StatusCodes.Status200OK)
                .Produces<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        }

        private static async Task<IResult> Handle(
        [FromQuery] long ISBN,
        HttpContext httpContext,
        IMediator mediator,
        CancellationToken cancellationToken)
        {

            Guid UserId = Guid.Parse(httpContext.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value);

            var response = await mediator.Send(new DeleteUserBookCommand(ISBN, UserId), cancellationToken);
            if (response.IsError)
            {
                return response.Errors.ToProblem();
            }

            return Results.Ok(response.Value);
        }
    }

    internal sealed record DeleteUserBookCommand(
        long ISBN,
        Guid UserId) : IRequest<ErrorOr<DeleteBookResponse>>;

    internal sealed class DeleteUserBookCommandHandler(
    IBookRepository bookRepository,
    ILogger<DeleteUserBookCommandHandler> logger)
        : IRequestHandler<DeleteUserBookCommand, ErrorOr<DeleteBookResponse>>
    {
        public async Task<ErrorOr<DeleteBookResponse>> Handle(DeleteUserBookCommand request, CancellationToken cancellationToken)
        {

            var book = await bookRepository.GetBookFromUserByISBN(request.UserId, request.ISBN);
            if (book is null)
            {
                logger.LogInformation("Book '{ISBN}' does not exists", request.ISBN);
                return Error.Conflict("Book does not exists");
            }

            await bookRepository.Delete(book);

            logger.LogInformation("Book deleted: {@Book} by user {@UserId}", request.ISBN, request.UserId);

            return book.MapToResponse();
        }
    }
}

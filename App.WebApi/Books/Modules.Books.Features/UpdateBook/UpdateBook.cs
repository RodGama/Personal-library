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
using Modules.Books.Features.RegisterBook;
using Modules.Books.Features.Shared.Responses;
using Modules.Books.Infrastructure.Database.Repositories.Interfaces;
using Modules.Common.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Books.Features.UpdateBook
{
    public sealed record UpdateBookRequest(
        string Title,
        string Author,
        string Description,
        long ISBN,
        string Publisher,
        string Genre,
        string ImageBase64,
        Guid UserId);

    public class UpdateBookEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/books/update", Handle)
                .WithName("UpdateBook")
                .WithTags("Books")
                .RequireAuthorization()
                .Produces<BookResponse>(StatusCodes.Status200OK)
                .Produces<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        }

        private static async Task<IResult> Handle(
        [FromBody] UpdateBookRequest request,
        HttpContext httpContext,
        IValidator<UpdateBookRequest> validator,
        IMediator mediator,
        CancellationToken cancellationToken)
        {

            Guid UserId = Guid.Parse(httpContext.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value);

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

    internal sealed record UpdateBookCommand(
        string Title,
        string Author,
        string Description,
        long ISBN,
        string Publisher,
        string Genre,
        string ImageBase64,
        Guid UserId) : IRequest<ErrorOr<BookResponse>>;

    internal sealed class UpdateBookCommandHandler(
    IBookRepository bookRepository,
    ILogger<UpdateBookCommandHandler> logger)
        : IRequestHandler<UpdateBookCommand, ErrorOr<BookResponse>>
    {
        public async Task<ErrorOr<BookResponse>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {

            var book = await bookRepository.GetBookFromUserByISBN(request.UserId, request.ISBN);
            if (book is null)
            {
                logger.LogInformation("Book '{ISBN}' does not exists", request.ISBN);
                return Error.Conflict("Book does not exists");
            }

            book = request.MapToBook(book);

            await bookRepository.Update(book);

            logger.LogInformation("Book altered: {@Book} by user {@UserId}", request.ISBN, request.UserId);

            return book.MapToResponse();
        }
    }
}

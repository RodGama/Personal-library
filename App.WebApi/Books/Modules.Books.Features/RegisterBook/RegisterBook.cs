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

namespace Modules.Books.Features.RegisterBook
{
    public sealed record RegisterBookRequest(
        string Title,
        string Author,
        string Description,
        long ISBN,
        string Publisher,
        string Genre,
        string ImageBase64,
        Guid UserId);

    public class RegisterBookEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/books/register", Handle)
                .WithName("RegisterBook")
                .WithTags("Books")
                .RequireAuthorization()
                .Produces<BookResponse>(StatusCodes.Status200OK)
                .Produces<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)
                .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        }

        private static async Task<IResult> Handle(
        [FromBody] RegisterBookRequest request,
        HttpContext httpContext,
        IValidator<RegisterBookRequest> validator,
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

    internal sealed record RegisterBookCommand(
        string Title,
        string Author,
        string Description,
        long ISBN,
        string Publisher,
        string Genre,
        string ImageBase64,
        Guid UserId) : IRequest<ErrorOr<BookResponse>>;

    internal sealed class RegisterBookCommandHandler(
    IBookRepository bookRepository,
    ILogger<RegisterBookCommandHandler> logger)
        : IRequestHandler<RegisterBookCommand, ErrorOr<BookResponse>>
    {
        public async Task<ErrorOr<BookResponse>> Handle(RegisterBookCommand request, CancellationToken cancellationToken)
        {
            var bookExists = await bookRepository.GetBookFromUserByISBN(request.UserId, request.ISBN);
            if (bookExists is not null)
            {
                logger.LogInformation("Book '{ISBN}' already exists", request.ISBN);
                return Error.Conflict("Book already exists");
            }

            var book = request.MapToBook();

            await bookRepository.Add(book);

            logger.LogInformation("Created book: {@Book}", book);

            return book.MapToResponse();
        }
    }
}

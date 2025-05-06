using Modules.Books.Domain;
using Modules.Books.Features.RegisterBook;
using Modules.Books.Features.Shared.Responses;


namespace Modules.Books.Features.UpdateBook
{
    internal static class UpdateBookMappingExtensions
    {
        public static UpdateBookCommand MapToCommand(this UpdateBookRequest request, Guid UserId)
           => new(request.Title,
                  request.Author,
                  request.Description,
                  request.ISBN,
                  request.Publisher,
                  request.Genre,
                  request.ImageBase64,
                  UserId);
        public static Book MapToBook(this UpdateBookCommand command, Book book)
        {
            book.ISBN = command.ISBN;
            book.UserId = command.UserId;
            book.Title = command.Title;
            book.Author = command.Author;
            book.Description = command.Description;
            book.Publisher = command.Publisher;
            book.Genre = command.Genre;
            book.ImageBase64 = command.ImageBase64;
            return book;
        }
        public static BookResponse MapToResponse(this Book book)
                 => new(
                    book.Title,
                    book.Author,
                    book.Description,
                    book.ISBN,
                    book.Publisher,
                    book.Genre, 
                    book.ImageBase64);
    }
}

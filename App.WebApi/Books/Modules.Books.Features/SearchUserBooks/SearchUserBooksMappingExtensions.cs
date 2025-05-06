using MediatR;
using Modules.Books.Domain;
using Modules.Books.Features.RegisterBook;
using Modules.Books.Features.Shared.Responses;


namespace Modules.Books.Features.SearchUserBooks
{
    internal static class SearchUserBooksMappingExtensions
    {
        public static List<BookResponse> MapToResponse(this List<Book> books)
            => books.Select(book => new BookResponse(
            book.Title,
            book.Author,
            book.Description,
            book.ISBN,
            book.Publisher,
            book.Genre,
            book.ImageBase64
            )).ToList();


    }
}

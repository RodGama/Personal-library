using Modules.Books.Domain;
using Modules.Books.Features.RetrieveUserBooks;
using Modules.Books.Features.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Modules.Books.Features.RetriveUserBooks
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

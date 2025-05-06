using Modules.Books.Domain;
using Modules.Books.Features.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Books.Features.RegisterBook
{
    internal static class RegisterBookBookMappingExtensions
    {
        public static RegisterBookCommand MapToCommand(this RegisterBookRequest request, Guid UserId)
           => new(request.Title,
               request.Author,
               request.Description,
               request.ISBN,
               request.Publisher,
               request.Genre,
               request.ImageBase64,
               UserId);

        public static Book MapToBook(this RegisterBookCommand command)
            => new Book
            {
                Title = command.Title,
                Author = command.Author,
                Description = command.Description,
                ISBN = command.ISBN,
                Publisher = command.Publisher,
                Genre = command.Genre,
                ImageBase64 = command.ImageBase64,
                UserId = command.UserId
            };

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

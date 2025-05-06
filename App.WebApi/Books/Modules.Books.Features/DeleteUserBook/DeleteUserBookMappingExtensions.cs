using Modules.Books.Domain;
using Modules.Books.Features.Shared.Responses;


namespace Modules.Books.Features.DeleteUserBook
{
    internal static class UpdateBookMappingExtensions
    {
        public static DeleteBookResponse MapToResponse(this Book book)
            => new(book.ISBN,
                   true);
    }
}

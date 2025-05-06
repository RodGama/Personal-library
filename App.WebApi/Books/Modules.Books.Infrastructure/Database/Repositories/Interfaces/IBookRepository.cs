using Modules.Books.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Books.Infrastructure.Database.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task Add(Book book);
        Task Delete(Book book);
        Task<List<Book>> Get(int PageNumber, int pageQuantity);
        Task<ICollection<Book>> GetAllBooksFromUserByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<ICollection<Book>> GetBooksByUserIdPagedAsync(Guid userId, int pageNumber, int pageQuantity, CancellationToken cancellationToken);
        Task<Book> GetBookFromUserByISBN(Guid userId, long ISBN);
        Task Update(Book book);
        Task<ICollection<Book>> SearchUserBooksAsync(Guid userId, string query, int pageNumber, int pageQuantity, CancellationToken cancellationToken);
    }
}

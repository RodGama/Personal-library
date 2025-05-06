using Microsoft.EntityFrameworkCore;
using Modules.Books.Domain;
using Modules.Books.Infrastructure.Database.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Books.Infrastructure.Database.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BooksDbContext _context;
        public BookRepository(BooksDbContext context)
        {
            _context = context;
        }
        public async Task Add(Book book)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Book book)
        {
            _context.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Book>> Get(int pageNumber, int pageQuantity)
        {
            var books = _context.Books.Skip(pageNumber * pageQuantity)
                        .Take(pageQuantity).ToList();
            return books;

        }

        public async Task<ICollection<Book>> GetAllBooksFromUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var books = _context.Books.Where(x => x.UserId == userId).ToList();
            return books;
        }

        public async Task<ICollection<Book>> GetBooksByUserIdPagedAsync(Guid userId, int pageNumber, int pageQuantity, CancellationToken cancellationToken)
        {
            var books = _context.Books.Where(x=>x.UserId==userId).Skip(pageNumber * pageQuantity)
                        .Take(pageQuantity).ToList();

            return books;
        }

        public async Task<Book> GetBookFromUserByISBN(Guid userId, long ISBN)
        {
            var book = await _context.Books.Where(x=>x.ISBN==ISBN && x.UserId==userId).FirstOrDefaultAsync();
            return book;
        }

        public async Task Update(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public Task<ICollection<Book>> SearchUserBooksAsync(Guid userId, string query, int pageNumber, int pageQuantity, CancellationToken cancellationToken)
        {
            var books = _context.Books
                .Where(x => x.UserId == userId && 
                (x.Title.Contains(query) 
                || x.Author.Contains(query)
                || x.Publisher.Contains(query)
                || x.Genre.Contains(query)
                || x.ISBN.ToString().Contains(query)
                ))
                .Skip(pageNumber * pageQuantity)
                .Take(pageQuantity)
                .ToList();
            return Task.FromResult<ICollection<Book>>(books);
        }
    }
}

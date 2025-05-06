using App.MVC.DTOs;

namespace App.MVC.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDTO>> GetBooksPagedAsync(string token, int PageNumber, int PageQuantity, bool IsSamePage);
        Task<IEnumerable<BookDTO>> GetAllBooksAsync(string token);
        Task<IEnumerable<BookDTO>> GetBookByQueryAsync(string token, string query,int PageNumber, int PageQuantity, bool IsSamePage);
        Task<List<string>> AddBookAsync(BookDTO book, string token);
        Task<BookDTO> UpdateBookAsync(BookDTO book, string token);
        Task<BookDTO> DeleteBookAsync(string token, string ISBN);
        Task<byte[]> GenerateReports(IEnumerable<BookDTO> books);
    }
}

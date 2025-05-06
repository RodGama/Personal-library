using App.MVC.DTOs;
using Microsoft.Extensions.Caching.Memory;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Reflection.Metadata;
using System.Text.Json;

namespace App.MVC.Services.Interfaces
{
    public class BookService : IBookService
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private HttpClient _httpClient = new HttpClient();

        public BookService(IMemoryCache cache, IConfiguration configuration)
        {
            _configuration = configuration;
            _cache = cache;
            _httpClient.BaseAddress = new Uri(_configuration["ApiBase:BaseAddress"]);
        }
        public async Task<List<string>> AddBookAsync(BookDTO book, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PutAsJsonAsync($"books/register", book);
            var body = await response.Content.ReadAsStringAsync();

            var result = JsonDocument.Parse(body);
            var list = new List<string>();
            if (result.RootElement.TryGetProperty("errors", out var errorsElement) && errorsElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var errorProperty in errorsElement.EnumerateObject())
                {
                    foreach (var errorMessage in errorProperty.Value.EnumerateArray())
                    {
                        list.Add(errorMessage.GetString());
                    }
                }

            }
            else
                list.Add("Book registered with success");

            return list;
        }

        public async Task<BookDTO> DeleteBookAsync(string token, string isbn)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"books/delete?isbn={isbn}");
            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BookDTO>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            _cache.Remove($"books:{token}");

            return result;
        }

        public async Task<IEnumerable<BookDTO>> GetBooksPagedAsync(string token, int PageNumber, int PageQuantity, bool IsSamePage)
        {
            IEnumerable<BookDTO> books = null;
            if (IsSamePage)
            {
                books = _cache.Get<IEnumerable<BookDTO>>($"books:{token}");
                if (books != null)
                {
                    return books;
                }
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"books/retrieve?PageNumber={PageNumber}&PageQuantity={PageQuantity}");
            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<List<BookDTO>>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            books = result;
            _cache.Set($"books:{token}", books, TimeSpan.FromMinutes(5));

            return books;
        }

        public async Task<IEnumerable<BookDTO>> GetAllBooksAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"books/all");
            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<List<BookDTO>>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result;
        }

        public async Task<IEnumerable<BookDTO>> GetBookByQueryAsync(string token, string query, int PageNumber, int PageQuantity, bool IsSamePage)
        {
            IEnumerable<BookDTO> books = null;
            
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"books/search?query={query}&pageNumber={PageNumber}&pageQuantity={PageQuantity}");
            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<List<BookDTO>>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            books = result;
            _cache.Set($"books:{token}", books, TimeSpan.FromMinutes(5));
            return books;
        }

        public async Task<BookDTO> UpdateBookAsync(BookDTO book, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PatchAsJsonAsync($"books/update", book);
            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<BookDTO>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            _cache.Remove($"books:{token}");
            return new BookDTO();
        }

        public async Task<byte[]> GenerateReports(IEnumerable<BookDTO> books)
        {
            using (var ms = new MemoryStream())
            {
                using (var pdf = new PdfDocument())
                {
                    PdfPage page = pdf.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont font = new XFont("Verdana", 12);

                    double yPosition = 100;

                    gfx.DrawString("Here are all your books!", font, XBrushes.Black, new XPoint(100, 50));

                    gfx.DrawLine(XPens.Black, 0, 70, page.Width, 70);

                    foreach (var book in books)
                    {
                        if (yPosition > page.Height - 100) 
                        {
                            page = pdf.AddPage();
                            gfx = XGraphics.FromPdfPage(page);
                            yPosition = 100; 
                        }
                        gfx.DrawString(book.Title + " by " + book.Author + " (" + book.Publisher + ")", font, XBrushes.Black, new XPoint(100, yPosition));
                        yPosition += 20; 
                        XFont synopsisFont = new XFont("Verdana", 10); 
                        gfx.DrawString(book.Description, synopsisFont, XBrushes.Black, new XPoint(100, yPosition));
                        yPosition += 40; 

                        gfx.DrawLine(XPens.Black, 100, yPosition, page.Width - 100, yPosition); 
                        yPosition += 20; 
                    }
                    pdf.Save(ms);
                }

                ms.Position = 0;
                return ms.ToArray(); 
            }
        }
    }
}

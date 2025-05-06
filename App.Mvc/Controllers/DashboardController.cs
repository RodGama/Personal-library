using App.MVC.DTOs;
using App.MVC.Extensions;
using App.MVC.Models.Forms;
using App.MVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using static System.Reflection.Metadata.BlobBuilder;

namespace App.MVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IBookService _bookService;
        private readonly IMemoryCache _cache;
        private int PageNumber = 0;
        private int PageQuantity = 10;
        private bool IsSamePage = false;
        public DashboardController(ILogger<DashboardController> logger, IBookService bookService, IMemoryCache cache)
        {
            _cache = cache;
            _logger = logger;
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? Search, string? page, string? lastPage)
        {
            var token = Request.Cookies["token"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Home", "Home");
            }

            if (!string.IsNullOrEmpty(page) && int.TryParse(page, out int pageNumber))
            {
                PageNumber = pageNumber;
            }

            if (lastPage == page)
                IsSamePage = true;

            ViewData["page"] = PageNumber.ToString();

            if (!string.IsNullOrEmpty(Search))
            {
                await _bookService.GetBookByQueryAsync(token, Search, PageNumber, PageQuantity, IsSamePage).ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        var books = task.Result;
                        ViewData["Books"] = books;
                        ViewData["search"] = Search;
                        _logger.LogInformation("Books retrieved successfully.");
                    }
                    else
                    {
                        _logger.LogInformation("Error retrieving books.");
                    }
                });

            }
            else
            {
                await _bookService.GetBooksPagedAsync(token, PageNumber, PageQuantity, IsSamePage).ContinueWith(task =>
                 {
                     if (task.IsCompletedSuccessfully)
                     {
                         var books = task.Result;
                         ViewData["Books"] = books;
                         _logger.LogInformation("Books retrieved successfully.");
                     }
                     else
                     {
                         _logger.LogInformation("Error retrieving books.");
                     }
                 });
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddBook()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(RegisterBookForm form)
        {
            var token = Request.Cookies["token"];

            if (ModelState.IsValid)
            {

                var base64String = ImageExtension.ConvertToBase64(form.File);
                BookDTO bookDTO = new BookDTO
                {
                    Title = form.Title,
                    Author = form.Author,
                    Genre = form.Genre.ToString(),
                    Description = form.Description,
                    ISBN = Int64.Parse(form.ISBN),
                    Publisher = form.Publisher,
                    ImageBase64 = base64String
                };

                var response = await _bookService.AddBookAsync(bookDTO, token);
                if (!response[0].Contains("success"))
                {
                    foreach (var error in response)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return View();
                }
                return RedirectToAction("Index");
            }

            foreach (var modelState in ModelState.Values)
            {
                foreach (ModelError modelError in modelState.Errors)
                {
                    ModelState.AddModelError(string.Empty, modelError.ErrorMessage);
                }
            }
            return View("AddBook");
        }

        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Home", "Home");
            }

            try
            {
                var books = await _bookService.GetAllBooksAsync(token);
                _logger.LogInformation("Books retrieved successfully.");

                var report = await _bookService.GenerateReports(books);
                _logger.LogInformation("Report generated successfully.");

                return File(report, "application/pdf", "BooksReport.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error generating report: {Message}", ex.Message);
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteBook(string ISBN)
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Home", "Home");
            }

            await _bookService.DeleteBookAsync(token, ISBN).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    var books = task.Result;
                    _logger.LogInformation("Book deleted successfully.");
                }
                else
                {
                    _logger.LogInformation("Error deleting books.");
                }
            });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditBook(string ISBN)
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Home", "Home");
            }

            var books = _cache.Get<IEnumerable<BookDTO>>($"books:{token}");

            if (books is null)
            {
                await _bookService.GetBookByQueryAsync(token, ISBN, PageNumber, PageQuantity, IsSamePage).ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        books = task.Result;
                        ViewData["book"] = books.Where(x => x.ISBN == Int64.Parse(ISBN)).FirstOrDefault();
                    }
                    else
                    {
                        _logger.LogInformation("Error retrieving books.");
                    }
                });
            }
            else
            {
                ViewData["book"] = books.Where(x => x.ISBN == Int64.Parse(ISBN)).FirstOrDefault();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBook(UpdateBookForm form)
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Home", "Home");
            }

            if (ModelState.IsValid)
            {
                if (form.BookCoverChanged)
                {
                    var base64String = ImageExtension.ConvertToBase64(form.File);
                    form.ImageBas64 = base64String;
                }
                BookDTO bookDTO = new BookDTO
                {
                    Title = form.Title,
                    Author = form.Author,
                    Genre = form.Genre.ToString(),
                    Description = form.Description,
                    ISBN = Int64.Parse(form.ISBN),
                    Publisher = form.Publisher,
                    ImageBase64 = form.ImageBas64
                };
                ViewData["book"] = bookDTO;

                await _bookService.UpdateBookAsync(bookDTO, token);
                return RedirectToAction("Index");
            }

            foreach (var modelState in ModelState.Values)
            {
                foreach (ModelError modelError in modelState.Errors)
                {
                    ModelState.AddModelError(string.Empty, modelError.ErrorMessage);
                }
            }
            return View("EditBook");
        }
    }
}

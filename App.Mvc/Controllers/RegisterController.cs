using App.MVC.DTOs;
using App.MVC.Models.Forms;
using App.MVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace App.MVC.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IUserService _userService;
        public RegisterController(ILogger<RegisterController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterUser(RegisterUserForm form)
        {
            if (ModelState.IsValid)
            {
                var userDTO = new UserDTO
                {
                    Name = form.Name,
                    Email = form.Email,
                    Password = form.Password,
                    BirthDate = form.BirthDate
                };

                var response = await _userService.RegisterUser(userDTO);
                if (!response[0].Contains("success"))
                {
                    foreach (var error in response)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    return View("Index");
                }

                await _userService.LoginUser(new LoginDTO
                {
                    Email = form.Email,
                    Password = form.Password
                }).ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                        {
                            var results = task.Result;
                            HttpContext.Response.Cookies.Append("token", results[1], new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.Strict,
                                Expires = DateTimeOffset.UtcNow.AddHours(2)
                            });
                            _logger.LogInformation("User logged successfully.");
                        }
                        else
                        {
                            _logger.LogInformation("Error loging user.");

                            // Handle the error
                        }
                    });

                return RedirectToAction("Index", "Dashboard");
            }
            foreach (var modelState in ModelState.Values)
            {
                foreach (ModelError modelError in modelState.Errors)
                {
                    ModelState.AddModelError(string.Empty, modelError.ErrorMessage);
                }
            }

            return View("Index");
        }
    }
}

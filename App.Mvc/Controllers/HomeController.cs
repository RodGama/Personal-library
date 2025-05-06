using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using App.MVC.Models;
using App.MVC.Services.Interfaces;
using App.MVC.DTOs;
using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Net.Http.Headers;
using App.MVC.Models.Forms;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserService _userService;
    public HomeController(ILogger<HomeController> logger, IUserService userService)
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
    public async Task<ActionResult> AuthenticateUser(LoginForm form)
    {
        if (ModelState.IsValid)
        {
            var loginDTO = new LoginDTO
            {
                Email = form.Email,
                Password = form.Password
            };

            var response = await _userService.LoginUser(loginDTO);
            if (!response[0].Contains("success"))
            {
                foreach (var error in response)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View("Index");
            }

            var token = response[1];
            HttpContext.Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            });

            return RedirectToAction("Index","Dashboard");
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

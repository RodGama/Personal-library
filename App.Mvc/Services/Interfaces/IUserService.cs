using App.MVC.DTOs;
using System.Text.Json;

namespace App.MVC.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<string>> RegisterUser(UserDTO request);
        Task<List<string>> LoginUser(LoginDTO request);
        Task<JsonDocument> UpdateUserAsync(UserDTO user);
    }
}

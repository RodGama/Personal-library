using App.MVC.DTOs;
using App.MVC.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace App.MVC.Services
{
    public class UserService : IUserService
    {
        private HttpClient _httpClient = new HttpClient();
        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiBase:BaseAddress"]);
        }
        public async Task<List<string>> LoginUser(LoginDTO request)
        {
            var list = new List<string>();
            await _httpClient.PostAsJsonAsync("users/login", request).ContinueWith(async task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    var response = task.Result;
                    var body = await response.Content.ReadAsStringAsync();

                    var result = JsonDocument.Parse(body);
                    
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
                    {
                        var token = result.RootElement.GetProperty("token").GetString();
                        list.Add("User logged with success");
                        list.Add(token);
                    }
                }
                else
                {
                    list.Add("Service not available. Try again later! ;)");
                }
            });

            if(list.Count==0)
                list.Add("Service not available. Try again later! ;)");
            return list;

        }
        public async Task<List<string>> RegisterUser(UserDTO userDTO)
        {
            var response = await _httpClient.PutAsJsonAsync("users/register", userDTO);
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
                list.Add("User registered with success");
            return list;
        }
        public async Task<JsonDocument> UpdateUserAsync(UserDTO user)
        {
            throw new NotImplementedException();
        }
    }
}

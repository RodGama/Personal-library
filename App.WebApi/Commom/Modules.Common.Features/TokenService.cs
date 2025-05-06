using Microsoft.IdentityModel.Tokens;
using Modules.Users.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Modules.Common.Features
{
    public class TokenService
    {
        public static string GenerateToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("ASPNETCORE_AUTO_RELOAD_WS_KEY"));

            var tokenConfig = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim("Token", user.Id.ToString()),
                ]),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenConfig);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public static AuthResponse DecryptToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken != null)
            {
                return new AuthResponse(
                    jsonToken.Claims.FirstOrDefault(c => c.Type == "Token").ToString(),
                    true);
            }
            // If the token is invalid, return an AuthResponse with IsValid set to false
            return new AuthResponse("",false);
        }
    }
}

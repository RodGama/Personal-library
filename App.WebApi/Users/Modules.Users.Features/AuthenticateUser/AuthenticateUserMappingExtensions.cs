using Modules.Common.Features;
using Modules.Users.Domain;
using Modules.Users.Features.RegisterUser;
using Modules.Users.Features.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Features.AuthenticateUser
{
    internal static class AuthenticateUserMappingExtensions
    {
        public static AuthenticateUserCommand MapToCommand(this AuthenticateUserRequest request)
            => new(request.Email,
                request.Password);

        public static AuthResponse MapToResponse(this AuthResponse auth)
            => new(
                auth.Token,
                auth.IsValid);
    }
}

using Modules.Common.Features;
using Modules.Users.Domain;
using Modules.Users.Features.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Features.RegisterUser
{
    internal static class RegisterUserMappingExtensions
    {
        public static RegisterUserCommand MapToCommand(this RegisterUserRequest request)
            => new(request.Email,
                request.Name,
                request.Password,
                request.BirthDate);

        public static User MapToUser(this RegisterUserCommand command)
            => new User
            {
                Email = command.Email,
                Name = command.Name,
                Password = BCryptor.Encrypt(command.Password),
                BirthDate = command.BirthDate
            };

        public static UserResponse MapToResponse(this User user)
            => new(
                user.Email,
                user.Name,
                user.Password,
                user.BirthDate);
    }
}

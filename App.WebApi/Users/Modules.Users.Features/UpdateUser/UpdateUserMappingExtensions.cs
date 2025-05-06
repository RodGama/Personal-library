using Modules.Users.Domain;
using Modules.Users.Features.Shared.Responses;
using Modules.Users.Features.UpdateUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Features.UpdateUser
{
    internal static class UpdateUserMappingExtensions
    {
        public static UpdateUserCommand MapToCommand(this UpdateUserRequest request, Guid UserId)
            => new(request.Password,
                UserId);

        public static UserResponse MapToResponse(this User user)
            => new(
                user.Email,
                user.Name,
                user.Password,
                user.BirthDate);
    }
}

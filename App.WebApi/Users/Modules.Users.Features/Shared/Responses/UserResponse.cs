using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Users.Features.Shared.Responses
{
    public sealed record UserResponse(
       string Email,
       string Name,
       string Password,
       DateOnly BirthDate);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Books.Features.Shared.Responses
{
    public sealed record DeleteBookResponse(
       long ISBN,
       bool IsDeleted);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Books.Features.Shared.Responses
{
    public sealed record BookResponse(
        string Title,
        string Author,
        string Description,
        long ISBN,
        string Publisher,
        string Genre,
        string ImageBase64);
}

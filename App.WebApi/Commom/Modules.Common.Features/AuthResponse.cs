﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Common.Features
{
    public sealed record AuthResponse(
      string Token,
      bool IsValid);
}

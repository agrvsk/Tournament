﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Shared.DTOs
{
    public record TokenDto(string AccessToken, string RefreshToken);

}

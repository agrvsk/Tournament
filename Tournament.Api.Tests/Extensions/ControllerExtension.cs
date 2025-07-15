﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Api.Tests.Extensions;

public static class ControllerExtension
{
    public static void SetUserIsAuth(this ControllerBase controller, bool isAuth, string role = "")
    {
        var identity = isAuth 
                        ? new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "TestUserId") }, "TestAuthType")
                        : new ClaimsIdentity();
        if(!string.IsNullOrEmpty(role))
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        var user = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = user };

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Tournament.Core.Entities;

public class User : IdentityUser
{
    public string? Name { get; set; }
    public string? Role { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? Expires { get; set; }
}

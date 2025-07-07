using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs
{
    public record UserForAuthDto([Required] string UserName, [Required] string PassWord);
}

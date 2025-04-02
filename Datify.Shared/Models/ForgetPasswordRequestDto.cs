using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datify.Shared.Models
{
    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; }
    }

    public class ResetPasswordRequestDto
    {
        public string Email { get; set; }
        public string ResetToken { get; set; }
        public string NewPassword { get; set; }
    }
}

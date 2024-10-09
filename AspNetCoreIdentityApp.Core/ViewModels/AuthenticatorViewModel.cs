using AspNetCoreIdentityApp.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class AuthenticatorViewModel
    {
        public string SharedKey { get; set; } = null!;
        public string? AuthenticatorUri { get; set; }

        [Display(Name = "Verification Code")]
        [Required(ErrorMessage ="Verification code cannot be empty.")]
        public string VerificationCode { get; set; } = null!;

        [Display(Name = "Two Factor Authentication Type")]
        public TwoFactor TwoFactorType { get; set; }
    }
}

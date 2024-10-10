using AspNetCoreIdentityApp.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class TwoFactorLoginViewModel
    {
        [Display(Name = "Verification Code")]
        [Required(ErrorMessage ="Verification code cannot be empty!")]
        
        public string VerificationCode { get; set; } = null!;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        [Display(Name = "If you will use a recovery code, tick the button.")]
        public bool isRecoveryCode { get; set; }

        public TwoFactor TwoFactorType { get; set; }
    }
}

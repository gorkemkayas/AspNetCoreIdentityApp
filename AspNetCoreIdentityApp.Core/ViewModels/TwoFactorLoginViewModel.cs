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
        [StringLength(8,ErrorMessage = "The verification code can be up to 8 characters long.")]
        public string VerificationCode { get; set; } = null!;

        public bool RememberMe { get; set; }

        public bool isRecoveryCode { get; set; }

        public TwoFactor TwoFactorType { get; set; }
    }
}

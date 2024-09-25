using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class ResetPasswordViewModel
    {

        [Required(ErrorMessage = "The 'Password' field cannot be left blank.")]
        [Display(Name = "Password :")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "The 'Password Confirm' field cannot be left blank.")]
        [Display(Name = "Password confirm :")]
        [Compare(nameof(NewPassword), ErrorMessage = "The passwords are not equal.")]
        [DataType(DataType.Password)]
        public string NewPasswordConfirm { get; set; } = null!;
    }
}

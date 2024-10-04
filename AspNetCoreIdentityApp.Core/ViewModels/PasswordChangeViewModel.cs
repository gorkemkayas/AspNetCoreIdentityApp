using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Required(ErrorMessage = "The 'Old Password' field cannot be left blank.")]
        [Display(Name = "Old Password :")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "The password should include least 5 letter.")]
        public string OldPassword { get; set; } = null!;

        [Required(ErrorMessage = "The 'New Password' field cannot be left blank.")]
        [Display(Name = "New Password :")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "The 'New Password' should include least 5 letter.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "The 'New Password Confirm' field cannot be left blank.")]
        [Display(Name = "New Password confirm :")]
        [Compare(nameof(NewPassword), ErrorMessage = "The passwords are not equal.")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "The 'New Password' should include least 5 letter.")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}

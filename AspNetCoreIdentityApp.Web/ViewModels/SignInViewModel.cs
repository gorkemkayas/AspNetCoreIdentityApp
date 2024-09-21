using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class SignInViewModel
    {

        [Required(ErrorMessage = "The 'Email' field cannot be left blank.")]
        [Display(Name = "Email :")]
        [EmailAddress(ErrorMessage = "Wrong Email format.")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "The 'Password' field cannot be left blank.")]
        [Display(Name = "Password :")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}

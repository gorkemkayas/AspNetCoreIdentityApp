using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "The 'Email' field cannot be left blank.")]
        [Display(Name = "Email :")]
        [EmailAddress(ErrorMessage = "Wrong Email format.")]
        public string Email { get; set; }
    }
}

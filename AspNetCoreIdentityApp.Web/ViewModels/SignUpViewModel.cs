using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class SignUpViewModel
    {
        public SignUpViewModel()
        {

        }
        public SignUpViewModel(string userName, string email, string phone, string password)
        {
            UserName = userName;
            Email = email;
            Phone = phone;
            Password = password;
        }
        [Required(ErrorMessage= "The 'Username' field cannot be left blank.")]
        [Display(Name = "Username :")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The 'Email' field cannot be left blank.")]
        [Display(Name = "Email :")]
        [EmailAddress(ErrorMessage ="Wrong Email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The 'Phone' field cannot be left blank.")]
        [Display(Name = "Phone :")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "The 'Password' field cannot be left blank.")]
        [Display(Name = "Password :")]
        [PasswordPropertyText]
        public string Password { get; set; }

        [Required(ErrorMessage = "The 'Password Confirm' field cannot be left blank.")]
        [Display(Name = "Password confirm :")]
        [Compare(nameof(Password),ErrorMessage = "The passwords are not equal.")]
        public string PasswordConfirm { get; set; }

    }
}

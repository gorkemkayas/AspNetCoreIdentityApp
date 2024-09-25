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
        [MinLength(4,ErrorMessage ="The username should include least 4 letter.")]
        [MaxLength(18, ErrorMessage = "The username should include least 4 letter.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The 'Email' field cannot be left blank.")]
        [Display(Name = "Email :")]
        [EmailAddress(ErrorMessage ="Wrong Email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The 'Phone' field cannot be left blank.")]
        [Display(Name = "Phone :")]
        [RegularExpression(@"^[0-9\s]+$", ErrorMessage = "Please enter just numbers.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "The 'Password' field cannot be left blank.")]
        [Display(Name = "Password :")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "The 'Password Confirm' field cannot be left blank.")]
        [Display(Name = "Password confirm :")]
        [Compare(nameof(Password),ErrorMessage = "The passwords are not equal.")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

    }
}

using AspNetCoreIdentityApp.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class UserEditViewModel
    {
        [Required(ErrorMessage = "The 'Username' field cannot be left blank.")]
        [Display(Name = "Username :")]
        [MinLength(4, ErrorMessage = "The username should include least 4 letter.")]
        [MaxLength(18, ErrorMessage = "The username should include least 4 letter.")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "The 'Email' field cannot be left blank.")]
        [Display(Name = "Email :")]
        [EmailAddress(ErrorMessage = "Wrong Email format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "The 'Phone' field cannot be left blank.")]
        [Display(Name = "Phone :")]
        [RegularExpression(@"^[0-9\s]+$", ErrorMessage = "Please enter just numbers.")]
        public string Phone { get; set; } = null!;

        [DataType(DataType.Date)]
        [Display(Name = "Birthdate :")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "City :")]
        public string? City { get; set; }

        [Display(Name = "Profile picture :")]
        public IFormFile? Picture { get; set; }

        [Display(Name = "Gender :")]
        public Gender? Gender { get; set; }
    }
}

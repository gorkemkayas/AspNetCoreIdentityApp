using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class RoleCreateViewModel
    {
        [Required(ErrorMessage = "The 'Name' field cannot be left blank.")]
        [Display(Name = "Role Name :")]
        public string Name { get; set; } = null!;
    }
}

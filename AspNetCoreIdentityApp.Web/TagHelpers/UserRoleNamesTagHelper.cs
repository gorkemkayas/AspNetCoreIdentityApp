using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace AspNetCoreIdentityApp.Web.TagHelpers
{
    public class UserRoleNamesTagHelper : TagHelper
    {
        public string UserId { get; set; } = null!;

        private readonly UserManager<AppUser> _userManager;

        public UserRoleNamesTagHelper(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var currentUser = await _userManager.FindByIdAsync(UserId) ?? throw new Exception("User not found in TagHelper!");
            var userRoles = await _userManager.GetRolesAsync(currentUser!);

            var stringBuilder = new StringBuilder();
            userRoles.ToList().ForEach(x=>
            {
                stringBuilder.Append(@$"<span class='badge bg-secondary me-1'>{x}</span>");
            });

            if(userRoles.Count()==0)
            {
                stringBuilder.Append(@$"<span class='badge bg-danger me-1'>No Role</span>");
            }

            output.Content.SetHtmlContent(stringBuilder.ToString());

        }
    }
}

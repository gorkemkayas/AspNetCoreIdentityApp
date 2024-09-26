using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Identity.Client;

namespace AspNetCoreIdentityApp.Web.TagHelpers
{
    public class UserPictureThumbnailTagHelper : TagHelper
    {
        public string? PictureUrl { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";

            if (string.IsNullOrEmpty(PictureUrl)) {
                output.Attributes.SetAttribute("src", "/userPictures/default_user_picture.png");
            }
            else
            {
                output.Attributes.SetAttribute("src", $"/userPictures/{PictureUrl}");
            }


            base.Process(context, output);
        }
    }
}

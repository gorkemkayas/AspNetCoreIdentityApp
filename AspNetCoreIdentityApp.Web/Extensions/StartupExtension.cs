using AspNetCoreIdentityApp.Web.Models;

namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class StartupExtension
    {
        public static void AddIdentityWithExtensions(this IServiceCollection service)
        {
            service.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghıijklmnoçpqrsştuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<AppDbContext>();
        }
    }
}

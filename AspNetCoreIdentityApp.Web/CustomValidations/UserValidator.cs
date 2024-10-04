using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {

            var errors = new List<IdentityError>();

            var isNumeric = int.TryParse(user.UserName,out _);
            var isStartNumeric = int.TryParse(user.UserName![0].ToString(), out _);

            if (isNumeric)
            {
                errors.Add(new() { Code = "UsernameConnotBeValue", Description = "Username cannot be value!" });
            }
            else if (isStartNumeric)
            {
                errors.Add(new() { Code = "UsernameConnotStartWithValue", Description = "Username cannot start with value!" });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
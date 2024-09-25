using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentityApp.Web.Controllers
{

    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            
            var userViewModel = new UserViewModel{ Email = currentUser.Email, PhoneNumber = currentUser.PhoneNumber, UserName = currentUser.UserName};

            return View(userViewModel);
        }
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();

        }

        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var hasUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            var checkOldPassword = await _userManager.CheckPasswordAsync(hasUser!, request.OldPassword);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Old password is not correct!");

                return View();
            }

            var result = await _userManager.ChangePasswordAsync(hasUser, request.OldPassword, request.NewPassword);

            if(!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(hasUser);

            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(hasUser, request.NewPassword, true, false);

            TempData["SucceedMessage"] = "Password updated successfully.";

            return View();
        }
    }
}

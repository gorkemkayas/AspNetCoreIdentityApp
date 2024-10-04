using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.Security.Claims;
using AspNetCoreIdentityApp.Core.Models;

namespace AspNetCoreIdentityApp.Web.Controllers
{

    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            var userViewModel = new UserViewModel { Email = currentUser!.Email, PhoneNumber = currentUser.PhoneNumber, UserName = currentUser.UserName, PictureUrl = currentUser.Picture };

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

            var result = await _userManager.ChangePasswordAsync(hasUser!, request.OldPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());

                return View();
            }

            await _userManager.UpdateSecurityStampAsync(hasUser!);

            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(hasUser!, request.NewPassword, true, false);

            TempData["SucceedMessage"] = "Password updated successfully.";

            return View();
        }

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser!.UserName!,
                Email = currentUser!.Email!,
                Phone = currentUser!.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender
            };

            return View(userEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            currentUser!.Email = request.Email;
            currentUser.City = request.City;
            currentUser.BirthDate = request.BirthDate;
            currentUser.UserName = request.UserName;
            currentUser.Gender = request.Gender;
            currentUser.PhoneNumber = request.Phone;


            if (request.Picture != null && request.Picture.Length > 0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";

                var newPicturePath = Path.Combine(wwwrootFolder.First(x=> x.Name == "userPictures").PhysicalPath!,randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);

                await request.Picture.CopyToAsync(stream);

                currentUser.Picture = randomFileName;


            }

            var updateToResult = await _userManager.UpdateAsync(currentUser);

            if (!updateToResult.Succeeded) {
                ModelState.AddModelErrorList(updateToResult.Errors);
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();

            if(request.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(currentUser, true, new[] { new Claim("Birthdate", request.BirthDate.Value.ToString())});
            }
            else
            {
                await _signInManager.SignInAsync(currentUser,true);
            }
            TempData["SucceedMessage"] = "User informations updated successfully.";

            var updatedUser = new UserEditViewModel()
            {
                UserName = currentUser.UserName,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Email = currentUser.Email,
                Gender = currentUser.Gender,
                Phone = currentUser.PhoneNumber
            };

            return View(updatedUser);
        }

        public IActionResult AccessDenied(string returnUrl)
        {

            string message = string.Empty;

            message = "You do not have permission to view this page. For permission, please contact your manager or HR.";
            ViewBag.Message = message;

            return View();
        }

        [HttpGet]
        public IActionResult Claims()
        {
            var claims = User.Claims.Select(x => new ClaimViewModel() { Issuer = x.Issuer, Type = x.Type , Value = x.Value}).ToList();

            return View(claims);
        }

        [Authorize(Policy = "CapitalCity")]
        public IActionResult CapitalCity()
        {
            return View();
        }

        [Authorize(Policy = "TrialClaim")]
        public IActionResult TrialPage()
        {
            return View();
        }

        [Authorize(Policy = "Violence")]
        public IActionResult ViolencePage()
        {
            return View();
        }

    }
}

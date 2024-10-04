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
using AspNetCoreIdentityApp.Service.Services;

namespace AspNetCoreIdentityApp.Web.Controllers
{

    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;
        private readonly IMemberService _memberService;
        private string UserName => User.Identity!.Name!;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider, IMemberService memberService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
            _memberService = memberService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _memberService.GetUserViewModelByUserNameAsync(UserName));
        }
        public async Task Logout()
        {
            await _memberService.LogoutAsync();
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

            if (!await _memberService.CheckPasswordAsync(UserName, request.OldPassword))
            {
                ModelState.AddModelError(string.Empty, "Old password is not correct!");

                return View();
            }

            var (isSuccess, errors) = await _memberService.ChangePasswordAsync(UserName, request.OldPassword, request.NewPassword);

            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors!);

                return View();
            }

            TempData["SucceedMessage"] = "Password updated successfully.";

            return View();
        }

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.Gender = _memberService.GetGenderSelectList();
            return View(await _memberService.GetUserEditViewModelAsync(UserName));
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            var (isSuccess,errors) = await _memberService.EditUser(request,UserName);

            if (!isSuccess) {
                ModelState.AddModelErrorList(errors!);
                return View();
            }

            TempData["SucceedMessage"] = "User informations updated successfully.";

            return View(await _memberService.GetUserEditViewModelAsync(UserName));
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
            return View(_memberService.GetClaims(User));
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

using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Diagnostics;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Services;


// ctlp pqvy ilov diyk password

//abcd


namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            var identityResult = await _userManager.CreateAsync(new() { UserName = request.UserName, PhoneNumber = request.Phone, Email = request.Email }, request.Password);


            if (identityResult.Succeeded)
            {
                TempData["SucceedMessage"] = "Membership process completed successfully.";
                return RedirectToAction(nameof(SignUp));
            }

            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());

            return View();


        }


        public IActionResult SignIn()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request, string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "The email or password is not correct.");
                return View();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, request.Password, request.RememberMe, true);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>()
                {"You will not able to log in for 3 minutes."});
                return View();
            }


            ModelState.AddModelErrorList(new List<string>() { $"The email or password is not correct.", $"Remaining login attempts: {4 - (int)(await _userManager.GetAccessFailedCountAsync(hasUser))}" });

            return View();
        }


        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            var hasUser = await _userManager.FindByEmailAsync(model.Email);

            if (hasUser == null) {
                ModelState.AddModelError(string.Empty, "No account was found associated with the email address you specified.");
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);
            var passwordResetLink = Url.Action("ResetPassword","Home", new {userId = hasUser.Id, Token = passwordResetToken}, HttpContext.Request.Scheme);

            await _emailService.SendResetPasswordEmail(passwordResetLink, hasUser.Email); // changed from 'model.Email' to 'hasUser.Email'

            TempData["SucceedMessage"] = "Your password reset link has been sent to your email address.";

            return RedirectToAction(nameof(ForgetPassword));

        }

        public async Task<IActionResult> ResetPassword(string userId,string Token)
        {
            TempData["userId"] = userId;
            TempData["token"] = Token;

            var hasUser = await _userManager.FindByIdAsync(userId);


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
        {
            var userId = TempData["userId"];
            var Token  = TempData["token"];

            if (userId == null && Token == null)
            {
                throw new Exception("An error occured.");
            }

            var hasUser = await _userManager.FindByIdAsync(userId!.ToString());

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, Token!.ToString(), request.NewPassword);

            if (result.Succeeded) {
                TempData["SucceedMessage"] = "Your password updated successfully!";
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

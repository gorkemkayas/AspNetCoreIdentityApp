using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Diagnostics;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Service.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using AspNetCoreIdentityApp.Core.Models;


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
        private readonly IMemberService _memberService;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, IMemberService memberService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _memberService = memberService;
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

            var identityResult = await _userManager.CreateAsync(new() { UserName = request.UserName, PhoneNumber = request.Phone, Email = request.Email, TwoFactor = 0}, request.Password);


            if (!identityResult.Succeeded)
            {
                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());

                return View();
            }


            var trialClaim = new Claim("TrialClaim", DateTime.Now.AddDays(7).ToString());

            var newUser = await _userManager.FindByNameAsync(request.UserName);
            var claimResult = await _userManager.AddClaimAsync(newUser!, trialClaim);

            if (!claimResult.Succeeded)
            {
                ModelState.AddModelErrorList(claimResult.Errors.Select(x => x.Description).ToList());

                return View();
            }

            TempData["SucceedMessage"] = "Membership process completed successfully.";
            return RedirectToAction(nameof(SignUp));

        }


        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "The email or password is not correct.");
                return View();
            }
            var (isSuccess,currentUser) = await _memberService.GetAppUserWithEmailAsync(request.Email);
            var isCorrectInfo = await _userManager.CheckPasswordAsync(currentUser!, request.Password);

            if(!isSuccess)
            {
                ModelState.AddModelError(string.Empty, "User is not found.");
                return View(request);
            }

            await _signInManager.SignOutAsync();
            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, request.Password, request.RememberMe, true);

            if(signInResult.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(TwoFactorLogin));
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>()
                {"You will not able to log in for 3 minutes."});
                return View();
            }
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelErrorList(new List<string>() { $"The email or password is not correct.", $"Remaining login attempts: {4 - (int)(await _userManager.GetAccessFailedCountAsync(hasUser))}" });
                return View();
            }

            if (hasUser.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(hasUser, request.RememberMe, new[] { new Claim("Birthdate", hasUser.BirthDate.Value.ToString()) });

            }

            await _userManager.ResetAccessFailedCountAsync(currentUser!);
            return Redirect(returnUrl!);
        }

        public async Task<IActionResult> TwoFactorLogin(string returnUrl = "/")
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            TempData["returnUrl"] = returnUrl;

            switch((TwoFactor)user!.TwoFactor!)
            {
                case TwoFactor.MicrosoftGoogle:

                    break;
            }

            return View(new TwoFactorLoginViewModel() { TwoFactorType=(TwoFactor)user.TwoFactor, isRecoveryCode = false, RememberMe = false,VerificationCode = string.Empty});

        }



        public IActionResult FacebookLogin(string returnUrl)
        {
            string redirectUrl = Url.Action("ExternalResponse", "Home", new { returnUrl = returnUrl })!;

            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);

            return new ChallengeResult("Facebook", properties);
        }
        public IActionResult GoogleLogin(string returnUrl)
        {
            string redirectUrl = Url.Action("ExternalResponse", "Home", new { returnUrl = returnUrl })!;

            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

            return new ChallengeResult("Google", properties);
        }

        public async Task<IActionResult> ExternalResponse(string returnUrl = "/")
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                return RedirectToAction("Login");
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);

            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }

            AppUser user = new AppUser();
            user.Email = info.Principal.FindFirst(ClaimTypes.Email)!.Value;
            string ExternalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            if(info.Principal.HasClaim(x => x.Type == ClaimTypes.Name))
            {
                string userName = info.Principal.FindFirst(ClaimTypes.Name)!.Value.Replace(" ", "-").ToLower() + ExternalUserId.Substring(0,5).ToString();

                user.UserName = userName;
            }
            else
            {
                user.UserName = info.Principal.FindFirst(ClaimTypes.Email)!.Value;
            }
            
            IdentityResult createResult = await _userManager.CreateAsync(user);

            if(createResult.Succeeded)
            {
                IdentityResult loginResult = await _userManager.AddLoginAsync(user,info);

                if(loginResult.Succeeded)
                {
                    await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,info.ProviderKey, isPersistent: true);
                    return Redirect(returnUrl ?? "/");
                }
            }

            return RedirectToAction("Error");

        }
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            var hasUser = await _userManager.FindByEmailAsync(model.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "No account was found associated with the email address you specified.");
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);
            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);

            await _emailService.SendResetPasswordEmail(passwordResetLink!, hasUser.Email!); // changed from 'model.Email' to 'hasUser.Email'

            TempData["SucceedMessage"] = "Your password reset link has been sent to your email address.";

            return RedirectToAction(nameof(ForgetPassword));

        }

        public async Task<IActionResult> ResetPassword(string userId, string Token)
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
            var Token = TempData["token"];

            if (userId == null && Token == null)
            {
                throw new Exception("An error occured.");
            }

            var hasUser = await _userManager.FindByIdAsync(userId!.ToString()!);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, Token!.ToString()!, request.NewPassword);

            if (result.Succeeded)
            {
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

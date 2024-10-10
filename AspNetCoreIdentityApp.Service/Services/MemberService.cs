using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Service.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFileProvider _fileProvider;

        public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IFileProvider fileProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserViewModel> GetUserViewModelByUserNameAsync(string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

            return new UserViewModel
            { 
                Email = currentUser!.Email, 
                PhoneNumber = currentUser.PhoneNumber, 
                UserName = currentUser.UserName, 
                PictureUrl = currentUser.Picture 
            };
            
        }
        public async Task<bool> CheckPasswordAsync(string userName, string password)
        {
            var hasUser = await _userManager.FindByNameAsync(userName);

            return await _userManager.CheckPasswordAsync(hasUser!, password);
        }

        public async Task<(bool,IEnumerable<IdentityError>?)> ChangePasswordAsync(string userName, string currentPassword, string newPassword)
        {
            var hasUser = await _userManager.FindByNameAsync(userName);
            var result = await _userManager.ChangePasswordAsync(hasUser!, currentPassword, newPassword);

            if (!result.Succeeded) 
            {
                return (false, result.Errors);
            }

            await _userManager.UpdateSecurityStampAsync(hasUser!);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(hasUser!, newPassword, true, false);

            return (true, null);
        }

        public async Task<UserEditViewModel> GetUserEditViewModelAsync(string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

            return new UserEditViewModel()
            {
                UserName = currentUser!.UserName!,
                Email = currentUser!.Email!,
                Phone = currentUser!.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender
            };
        }

        public SelectList GetGenderSelectList()
        {
            return new SelectList(Enum.GetNames(typeof(Gender)));
        }

        public async Task<(bool, IEnumerable<IdentityError>?)> EditUser(UserEditViewModel request, string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

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

                var newPicturePath = Path.Combine(wwwrootFolder.First(x => x.Name == "userPictures").PhysicalPath!, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);

                await request.Picture.CopyToAsync(stream);

                currentUser.Picture = randomFileName;
            }

            var updateToResult = await _userManager.UpdateAsync(currentUser);

            if(!updateToResult.Succeeded)
            {
                return (false, updateToResult.Errors);
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();

            if (request.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(currentUser, true, new[] { new Claim("Birthdate", request.BirthDate.Value.ToString()) });
            }
            else
            {
                await _signInManager.SignInAsync(currentUser, true);
            }

            return (true, null);
        }

        public List<ClaimViewModel> GetClaims(ClaimsPrincipal principal)
        {
            var claims = principal.Claims.Select(x => new ClaimViewModel() { Issuer = x.Issuer, Type = x.Type, Value = x.Value }).ToList();
            return claims;
        }

        public async Task<(bool,AppUser?)> GetAppUserWithEmailAsync(string email) 
        {
            var currentUser = await _userManager.FindByEmailAsync(email);
            if (currentUser == null) 
            {
                return (false, null);
            }
            else
            {
                return (true,currentUser);
            }
        }
    }
}

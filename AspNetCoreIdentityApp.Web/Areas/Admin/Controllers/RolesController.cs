using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class RolesController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(x => new RoleViewModel() {
                Name = x.Name!, 
                Id = x.Id}).ToListAsync();

            return View(roles);
        }


        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name});

            if (!result.Succeeded) 
            { 
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }

            TempData["SucceedMessage"] = $"The role named as '{request.Name} is created successfully!'";
            return RedirectToAction(nameof(RolesController.Index));    
        }

        public async Task<IActionResult> RoleUpdate(string Id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(Id);

            if(roleToUpdate == null)
            {
                throw new Exception("The role you specified could not be found.");
            }

            return View(new RoleUpdateViewModel() {Id =roleToUpdate!.Id, Name = roleToUpdate!.Name!});
        }

        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
        {

            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);

            string oldRole = roleToUpdate!.Name!;

            roleToUpdate!.Name = request.Name;

            var result = await _roleManager.UpdateAsync(roleToUpdate);

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }

            TempData["SucceedMessage"] = $"The role named as '{oldRole}' updated to '{roleToUpdate.Name}'";
            return RedirectToAction(nameof(RolesController.Index));
        }

        public async Task<IActionResult> RoleDelete(string Id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(Id);
            if(roleToDelete == null)
            {
                throw new Exception("The role you specified could not be found.");
            }

            var result = await _roleManager.DeleteAsync(roleToDelete);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.Select(x => x.Description).First());
            }

            TempData["SucceedMessage"] = $"The role named as '{roleToDelete.Name} is deleted successfully!'";
            return RedirectToAction(nameof(RolesController.Index));
        }

        public async Task<IActionResult> AssignRoleToUser(string Id)
        {
            ViewBag.UserId = Id;
            var currentUser = await _userManager.FindByIdAsync(Id) ?? throw new Exception("The User not found.");
            var roles = await _roleManager.Roles.AsNoTracking().ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            var roleViewModelList = new List<AssignRoleToUserViewModel>();

            foreach (var role in roles)
            {
                var assignRoleToViewModel = new AssignRoleToUserViewModel() { Id = role.Id, Name = role.Name!};

                if (userRoles.Contains(role.Name!)) 
                { 
                assignRoleToViewModel.Exist = true;
                }

                roleViewModelList.Add(assignRoleToViewModel);
            }

            return View(roleViewModelList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(List<AssignRoleToUserViewModel> requestList, string UserId)
        {
            var currentUser = await _userManager.FindByIdAsync(UserId) ?? throw new Exception("The User not found.");

            foreach (var role in requestList)
            {
                if (role.Exist)
                {
                    await _userManager.AddToRoleAsync(currentUser,role.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(currentUser,role.Name);
                }
            }

            // Update security stamp for security.
            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(currentUser, isPersistent: true);

            return RedirectToAction(nameof(HomeController.UserList),"Home");
        }
    }
}

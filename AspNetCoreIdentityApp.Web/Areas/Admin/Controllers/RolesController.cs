using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
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
    }
}

using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Seeds
{
    public class PermissionSeed
    {
        public static async Task Seed(RoleManager<AppRole> roleManager)
        {
            var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");
            var hasAdvancedRole = await roleManager.RoleExistsAsync("AdvancedRole");
            var hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");

            if (!hasBasicRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "BasicRole" });

                var basicRole = (await roleManager.FindByNameAsync("BasicRole"))!;

                await AddReadPermission(basicRole,roleManager);
            }
            if (!hasAdvancedRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "AdvancedRole" });

                var basicRole = (await roleManager.FindByNameAsync("AdvancedRole"))!;

                await AddReadPermission(basicRole, roleManager);
                await AddUpdatePermission(basicRole,roleManager);
                await AddCreatePermission(basicRole,roleManager);
            }
            if (!hasAdminRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "AdminRole" });

                var basicRole = (await roleManager.FindByNameAsync("AdminRole"))!;

                await AddReadPermission(basicRole, roleManager);
                await AddUpdatePermission(basicRole, roleManager);
                await AddCreatePermission(basicRole, roleManager);
                await AddDeletePermission(basicRole,roleManager);
            }
        }
        public static async Task AddReadPermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permissions.Stock.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permissions.Order.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permissions.Catalog.Read));
        }
        public static async Task AddUpdatePermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permissions.Stock.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permissions.Order.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permissions.Catalog.Update));
        }
        public static async Task AddCreatePermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permissions.Stock.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permissions.Order.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permissions.Catalog.Create));
        }
        public static async Task AddDeletePermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permissions.Stock.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permissions.Order.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Core.PermissionRoot.Permissions.Catalog.Delete));
        }

    }
}

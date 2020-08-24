using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Data
{
    public static class SeedData
    {
        public async static Task Seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }

        private async static Task SeedUsers(UserManager<IdentityUser> userManager)
        {

            if(await userManager.FindByEmailAsync("admin@fantics.cloud") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@fantics@cloud",
                };
                var result = await userManager.CreateAsync(user, "P@ssw0rd1");
                if(result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }
            if (await userManager.FindByEmailAsync("customer1@fantics.cloud") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "customer1",
                    Email = "customer1@fantics@cloud",
                };
                var result = await userManager.CreateAsync(user, "P@ssw0rd1");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
            if (await userManager.FindByEmailAsync("customer2@fantics.cloud") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "customer2",
                    Email = "customer2@fantics@cloud",
                };
                var result = await userManager.CreateAsync(user, "P@ssw0rd1");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
        }

        private async static Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if(!await roleManager.RoleExistsAsync("Administrator"))
            {
                var role = new IdentityRole
                {
                    Name = "Administrator"
                };
                await roleManager.CreateAsync(role);
            }
            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                var role = new IdentityRole
                {
                    Name = "Customer"
                };
                await roleManager.CreateAsync(role);
            }
        }
    }
}

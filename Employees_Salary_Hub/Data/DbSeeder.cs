using Employees_Salary_Hub.Models;
using Microsoft.AspNetCore.Identity;

namespace Employees_Salary_Hub.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // ── Seed Roles ────────────────────────────────────────
            string[] roles = { "Admin", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // ── Seed Admin User ───────────────────────────────────
            const string adminCode = "ADMIN001";
            const string adminPassword = "Admin@1234";

            if (await userManager.FindByNameAsync(adminCode) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminCode,
                    EmployeeCode = adminCode,
                    FullName = "System Administrator",
                    Email = "mail2mr.ukashyap@gmail.com",
                    Department = "IT",
                    Designation = "Administrator",
                    EmailConfirmed = true,
                    IsFirstLogin = false,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
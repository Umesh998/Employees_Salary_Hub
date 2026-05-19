//using Employees_Salary_Hub.Models;
//using Microsoft.AspNetCore.Identity;


//namespace Employees_Salary_Hub.Data
//{
//    public static class DataSeeder
//    {
//        public static async Task SeedAsync(
//            IServiceProvider serviceProvider)
//        {
//            var roleManager = serviceProvider
//                .GetRequiredService<RoleManager<IdentityRole>>();
//            var userManager = serviceProvider
//                .GetRequiredService<UserManager<ApplicationUser>>();

//            // ── Create Roles ──────────────────────────────────────
//            string[] roles = { "Admin", "Employee" };
//            foreach (var role in roles)
//                if (!await roleManager.RoleExistsAsync(role))
//                    await roleManager.CreateAsync(new IdentityRole(role));

//            // ── Create Default Admin ──────────────────────────────
//            const string adminCode = "ADMIN001";
//            if (await userManager.FindByNameAsync(adminCode) == null)
//            {
//                var admin = new ApplicationUser
//                {
//                    UserName = adminCode,
//                    EmployeeCode = adminCode,
//                    FullName = "System Administrator",
//                    MobileNumber = "+91XXXXXXXXXX",
//                    Email = "admin@company.com",
//                    IsFirstLogin = false,
//                    IsActive = true
//                };
//                var result = await userManager.CreateAsync(admin, "Admin@123456!");
//                if (result.Succeeded)
//                    await userManager.AddToRoleAsync(admin, "Admin");
//            }
//        }
//    }
//}


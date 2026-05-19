// Helpers/PasswordValidator.cs
using Microsoft.AspNetCore.Identity;

namespace Employees_Salary_Hub.Helpers
{
    public class CustomPasswordValidator<TUser> : IPasswordValidator<TUser>
        where TUser : class
    {
        public async Task<IdentityResult> ValidateAsync(
            UserManager<TUser> manager, TUser user, string? password)
        {
            var errors = new List<IdentityError>();
            if (string.IsNullOrEmpty(password)) return IdentityResult.Failed(
                new IdentityError { Description = "Password is required." });

            if (password.Length < 8)
                errors.Add(new IdentityError { Description = "Min 8 characters." });
            if (!password.Any(char.IsUpper))
                errors.Add(new IdentityError { Description = "At least 1 uppercase letter." });
            if (!password.Any(char.IsDigit) || password.Count(char.IsDigit) < 2)
                errors.Add(new IdentityError { Description = "At least 2 numbers." });
            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                errors.Add(new IdentityError { Description = "At least 1 special character." });
            return errors.Count == 0
    ? IdentityResult.Success
    : IdentityResult.Failed(errors.ToArray());
        }
    }
}



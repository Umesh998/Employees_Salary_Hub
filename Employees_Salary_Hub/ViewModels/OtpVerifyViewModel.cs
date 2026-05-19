using System.ComponentModel.DataAnnotations;

namespace Employees_Salary_Hub.ViewModels
{
    public class VerifyOTPViewModel
    {
        public class OtpVerifyViewModel
        {
            [Required]
            public string UserId { get; set; } = string.Empty;

            [Required]
            public string OtpCode { get; set; } = string.Empty;
        }
    }
}

using Employees_Salary_Hub.Models;

namespace Employees_Salary_Hub.Models
{
    public class OtpRecord
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
        public bool IsUsed { get; set; } = false;
        public int Attempts { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser User { get; set; } = null!;
    }
}


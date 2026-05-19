namespace Employees_Salary_Hub.Service.Interfaces
{
    public interface IOtpService
    {
        Task GenerateAndSendAsync(string userId, string mobile);
        Task<bool> VerifyAsync(string userId, string code);
    }
}

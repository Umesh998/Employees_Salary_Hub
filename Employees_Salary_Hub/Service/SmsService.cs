using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Employees_Salary_Hub.Service.Interfaces;

namespace Employees_Salary_Hub.Services
{
    public class SmsService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IConfiguration config, ILogger<SmsService> logger)
        { _config = config; _logger = logger; }

        public async Task SendAsync(string toNumber, string message)
        {
            try
            {
                var accountSid = _config["SMS:AccountSid"]!;
                var authToken = _config["SMS:AuthToken"]!;
                TwilioClient.Init(accountSid, authToken);

                var msg = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(_config["SMS:FromNumber"]),
                    to: new Twilio.Types.PhoneNumber(toNumber));

                _logger.LogInformation("SMS sent to {To}: {Sid}", toNumber, msg.Sid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS to {Number}", toNumber);
                throw;
            }
        }

        public Task SendAsync(string toEmail, string subject, string body)
        {
            throw new NotImplementedException();
        }
    }
}

//using System.Net;
//using System.Net.Mail;
//using Employees_Salary_Hub.Service.Interfaces;

//namespace Employees_Salary_Hub.Services
//{
//    public class EmailService : IEmailService
//    {
//        private readonly IConfiguration _config;
//        private readonly ILogger<EmailService> _logger;

//        public EmailService(IConfiguration config, ILogger<EmailService> logger)
//        {
//            _config = config;
//            _logger = logger;
//        }

//        //public async Task SendAsync(string toEmail, string subject, string body)
//        //{
//        //    var host = _config["Email:Host"]!;
//        //    var port = int.Parse(_config["Email:Port"]!);
//        //    var username = _config["Email:Username"]!;
//        //    var password = _config["Email:Password"]!;
//        //    var fromName = _config["Email:FromName"] ?? "Salary Hub";

//        //    using var client = new SmtpClient(host, port)
//        //    {
//        //        Credentials = new NetworkCredential(username, password),
//        //        EnableSsl = true
//        //    };

//        //    var mail = new MailMessage
//        //    {
//        //        From = new MailAddress(username, fromName),
//        //        Subject = subject,
//        //        Body = body,
//        //        IsBodyHtml = true
//        //    };
//        //    mail.To.Add(toEmail);

//        //    await client.SendMailAsync(mail);
//        //    _logger.LogInformation("Email OTP sent to {Email}", toEmail);
//        //}




//        public async Task SendAsync(string toEmail, string subject, string body)
//        {
//            try
//            {
//                var host = _config["Email:Host"]!;
//                var port = int.Parse(_config["Email:Port"]!);
//                var username = _config["Email:Username"]!;
//                var password = _config["Email:Password"]!;
//                var fromName = _config["Email:FromName"] ?? "Salary Hub";

//                _logger.LogInformation("Sending email to {Email} via {Host}:{Port}",
//                    toEmail, host, port);

//                using var client = new SmtpClient(host, port)
//                {
//                    Credentials = new NetworkCredential(username, password),
//                    EnableSsl = true
//                };

//                var mail = new MailMessage
//                {
//                    From = new MailAddress(username, fromName),
//                    Subject = subject,
//                    Body = body,
//                    IsBodyHtml = true
//                };
//                mail.To.Add(toEmail);

//                await client.SendMailAsync(mail);
//                _logger.LogInformation("✅ Email sent successfully to {Email}", toEmail);
//            }
//            catch (Exception ex)
//            {
//                // This will show the exact error in Visual Studio Output
//                _logger.LogError(ex, "❌ Failed to send email to {Email}", toEmail);
//                throw;
//            }
//        }
//    }
//}












using System.Net;
using System.Net.Mail;
using Employees_Salary_Hub.Service.Interfaces;

namespace Employees_Salary_Hub.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            // ── Read config ───────────────────────────────────────
            var host = _config["Email:Host"];
            var portStr = _config["Email:Port"];
            var username = _config["Email:Username"];
            var password = _config["Email:Password"];
            var fromName = _config["Email:FromName"] ?? "Salary Hub";

            // ── Log everything for debugging ──────────────────────
            _logger.LogInformation("=== EMAIL DEBUG ===");
            _logger.LogInformation("Host:     {Host}", host ?? "NULL");
            _logger.LogInformation("Port:     {Port}", portStr ?? "NULL");
            _logger.LogInformation("Username: {Username}", username ?? "NULL");
            _logger.LogInformation("Password: {Password}", string.IsNullOrEmpty(password)
                ? "NULL/EMPTY" : $"SET ({password.Length} chars)");
            _logger.LogInformation("To:       {To}", toEmail ?? "NULL");
            _logger.LogInformation("===================");

            // ── Validate before sending ───────────────────────────
            if (string.IsNullOrEmpty(host))
                throw new InvalidOperationException(
                    "Email:Host is missing from appsettings.json");
            if (string.IsNullOrEmpty(portStr) || !int.TryParse(portStr, out var port))
                throw new InvalidOperationException(
                    "Email:Port is missing or invalid in appsettings.json");
            if (string.IsNullOrEmpty(username))
                throw new InvalidOperationException(
                    "Email:Username is missing from appsettings.json");
            if (string.IsNullOrEmpty(password))
                throw new InvalidOperationException(
                    "Email:Password is missing from appsettings.json");
            if (string.IsNullOrEmpty(toEmail))
                throw new InvalidOperationException(
                    "Recipient email (toEmail) is null or empty");

            try
            {
                _logger.LogInformation(
                    "Connecting to SMTP {Host}:{Port}...", host, port);

                using var client = new SmtpClient(host, port)
                {
                    // Strip spaces from app password just in case
                    Credentials = new NetworkCredential(username, password.Replace(" ", "")),
                    EnableSsl = true,
                    Timeout = 10000
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(username, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(toEmail);

                await client.SendMailAsync(mail);

                _logger.LogInformation(
                    "✅ Email sent successfully to {Email}", toEmail);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex,
                    "❌ SMTP error — StatusCode: {Code} | Message: {Msg}",
                    ex.StatusCode, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Unexpected error sending email to {Email} | {Msg}",
                    toEmail, ex.Message);
                throw;
            }
        }
    }
}


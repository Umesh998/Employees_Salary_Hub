using Employees_Salary_Hub.Data;
using Employees_Salary_Hub.Models;
using Employees_Salary_Hub.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Twilio.TwiML.Messaging;

namespace Employees_Salary_Hub.Services
{
    public class OtpService : IOtpService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService; // ← FIX: was commented out as ISmsService
        private readonly IConfiguration _config;
        private readonly ILogger<OtpService> _logger;

        public OtpService(ApplicationDbContext context,
            IEmailService emailService, IConfiguration config, ILogger<OtpService> logger)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
            _logger = logger;
        }

      

        public async Task GenerateAndSendAsync(string userId, string email)
        {
            // Invalidate old unused OTPs for this user
            var old = await _context.OtpRecords
                .Where(o => o.UserId == userId && !o.IsUsed)
                .ToListAsync();
            old.ForEach(o => o.IsUsed = true);
            await _context.SaveChangesAsync();

            var expiryMinutes = int.Parse(_config["OTP:ExpiryMinutes"] ?? "5");

            var otp = new OtpRecord
            {
                UserId = userId,
                OtpCode = GenerateCode(),
                ExpiryTime = DateTime.UtcNow.AddMinutes(expiryMinutes),
                IsUsed = false
            };

            await _context.OtpRecords.AddAsync(otp);
            await _context.SaveChangesAsync();

            // Send OTP via Email
            await _emailService.SendAsync(
                email,
                "Your Salary Hub OTP Code",
                $@"<div style='font-family:Arial,sans-serif;max-width:400px;margin:auto;
                              padding:24px;border:1px solid #e0e0e0;border-radius:8px;'>
                    <h2 style='color:#1a73e8;'>Employees Salary Hub</h2>
                    <p>Your One-Time Password (OTP) is:</p>
                    <h1 style='letter-spacing:8px;color:#333;'>{otp.OtpCode}</h1>
                    <p>Valid for <b>{expiryMinutes} minutes</b>. Do not share this with anyone.</p>
                    <hr style='border:none;border-top:1px solid #eee;'/>
                    <p style='font-size:12px;color:#999;'>
                        If you did not request this, please ignore this email.
                    </p>
                   </div>"
            );

        }

        //public async Task<bool> VerifyAsync(string userId, string code)
        //{
        //    var maxAttempts = int.Parse(_config["OTP:MaxAttempts"] ?? "3");

        //    var otp = await _context.OtpRecords
        //        .Where(o => o.UserId == userId && !o.IsUsed
        //                 && o.OtpCode == code
        //                 && o.ExpiryTime > DateTime.UtcNow)
        //        .FirstOrDefaultAsync();

        //    if (otp == null)
        //    {
        //        // Increment attempts on the latest active OTP
        //        var latest = await _context.OtpRecords
        //            .Where(o => o.UserId == userId && !o.IsUsed)
        //            .OrderByDescending(o => o.CreatedAt)
        //            .FirstOrDefaultAsync();

        //        if (latest != null)
        //        {
        //            latest.Attempts++;
        //            if (latest.Attempts >= maxAttempts)
        //                latest.IsUsed = true; // invalidate after max attempts
        //            await _context.SaveChangesAsync();
        //        }
        //        return false;
        //    }

        //    otp.IsUsed = true;
        //    await _context.SaveChangesAsync();

        //    // ← Add this line temporarily for testing
        //    _logger.LogWarning("🔑 OTP for {UserId}: {Code}", userId, otp.OtpCode);
        //    return true;


        //}




        public async Task<bool> VerifyAsync(string userId, string code)
        {
            // ← Log what we received at the very start
            _logger.LogWarning("🔍 VerifyAsync called — UserId: '{UserId}' Code: '{Code}'", userId, code);

            var maxAttempts = int.Parse(_config["OTP:MaxAttempts"] ?? "3");

            var otp = await _context.OtpRecords
                .Where(o => o.UserId == userId && !o.IsUsed
                         && o.OtpCode == code
                         && o.ExpiryTime > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            // ← Log what's in the database for this user
            var allOtps = await _context.OtpRecords
                .Where(o => o.UserId == userId)
                .ToListAsync();
            foreach (var r in allOtps)
            {
                _logger.LogWarning("📋 DB Record — Code: '{Code}' IsUsed: {Used} Expiry: {Exp}",
                    r.OtpCode, r.IsUsed, r.ExpiryTime);
            }

            if (otp == null)
            {
                _logger.LogWarning("❌ OTP not matched for UserId: '{UserId}'", userId);

                var latest = await _context.OtpRecords
                    .Where(o => o.UserId == userId && !o.IsUsed)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();

                if (latest != null)
                {
                    latest.Attempts++;
                    if (latest.Attempts >= maxAttempts)
                        latest.IsUsed = true;
                    await _context.SaveChangesAsync();
                }
                return false;
            }

            otp.IsUsed = true;
            await _context.SaveChangesAsync();
            _logger.LogWarning("✅ OTP verified successfully for UserId: '{UserId}'", userId);
            return true;
        }

        private static string GenerateCode() =>
            new Random().Next(100000, 999999).ToString();
    }
}

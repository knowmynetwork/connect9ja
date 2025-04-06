using Datify.API.Data;
using Datify.Shared.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace Datify.API.Services;

public class OtpService(ApplicationDbContext context, IEmailService emailService) : IOtpService
{
    public string GenerateOtp()
    {
        return new Random().Next(100000, 999999).ToString();
    }

    public async Task SaveOtpAsync(string contact, ContactType contactType)
    {
        var verification = new OtpVerification()
        {
            ContactNumberOrEmailValue = contact,
            OtpCode = GenerateOtp(),
            ContactType = contactType,
            ExpirationTime = DateTime.UtcNow.AddMinutes(15) // OTP expires in 15 minutes
        };

        context.OtpVerifications.Add(verification);
        await context.SaveChangesAsync();

        await SendOtpToUser(contact, verification.OtpCode);
    }

    public async Task SendOtpToUser(string contact, string verificationOtpCode)
    {
        //if email
        await emailService.SendEmailAsync("noreply@homxly.com", contact, "One Time Password (OTP)",
            $"<p>This is your OTP <strong> {verificationOtpCode} </strong> to continue Homxly registration</p>");
    }

    public async Task<bool> VerifyOtpAsync(string contact, string otp)
    {
        var verification = await context.OtpVerifications.OrderByDescending(x => x.Id).FirstAsync(x =>
            x.ContactNumberOrEmailValue == contact);

        if (verification == null || verification.OtpCode != otp.Trim())
            throw new InvalidOperationException(
                "otp code not verified. Either an old otp code or time has expired. Try again or request a new one");

        verification.IsVerified = true;
        verification.DateVerified = DateTime.UtcNow;
        context.OtpVerifications.Update(verification);
        await context.SaveChangesAsync();
        return true;
    }
}
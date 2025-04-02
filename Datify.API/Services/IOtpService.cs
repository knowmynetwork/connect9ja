using Datify.API.Contracts;
using Datify.Shared.Models.Enum;

namespace Datify.API.Services
{
    public interface IOtpService : IServices
    {
        public Task SaveOtpAsync(string contact, ContactType contactType);
        public Task <bool> VerifyOtpAsync(string contact, string otp);
        public  Task SendOtpToUser(string contact, string verificationOtpCode);
        public string GenerateOtp();
    }
}

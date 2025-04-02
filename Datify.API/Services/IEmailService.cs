using Datify.API.Contracts;


namespace Datify.API.Services
{
    public interface IEmailService : IServices
    {
          public  Task SendEmailAsync(string from, string to, string subject, string body);
          public  Task SendEmailAsync(string to, string subject, string body);
        public Task SmtpSendEmailAsync(string recipientEmail, string subject, string body);
    }
}

using Datify.API.Configuration;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
//using System.Net.Mail;
using MailKit.Net.Smtp;
namespace Datify.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;


        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;


        }

        public async Task SmtpSendEmailAsync(string recipientEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Homxly", _emailSettings.SenderEmail));
            email.To.Add(new MailboxAddress("", recipientEmail));
            email.Subject = subject;
            email.Body = new TextPart("plain") { Text = body };

            using var smtp = new SmtpClient();
            try
            {
                
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.UserName, _emailSettings.SenderPassword);
               var response = await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                throw;
            }
        }

        public async Task SendEmailAsync(string from, string to, string subject, string body)
        {
            await SendEmail(to, subject, body, from);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await SendEmail(to, subject, body);
        }

        private async Task SendEmail(string to, string subject, string body, string? from = null)
        {
            var htmlBody = EmailTemplate.ComposeTheEmail(body);
            try
            {
                //call the smtp method here
                await SmtpSendEmailAsync(to, "Homxly: " + subject, htmlBody);

                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }

    public class EmailTemplate
    {
        private const string HtmlTemplate = @"
        <html>
        <head>
            <link href='https://fonts.googleapis.com/css2?family=Nunito:wght@400;700&display=swap' rel='stylesheet'>
            <style>
                body {{
                    font-family: 'Nunito', sans-serif;
                    line-height: 1.6;
                }}
                .content {{
                    margin: 20px;
                }}
            </style>
        </head>
        <body>
            <div class='content'>
                {0}
                <br/>
                <p>Best Regards,</p>
                <p>The Homxly Team</p>
                <p>Designed By Homxly Limited</p>
            </div>
        </body>
        </html>";

        public string ComposeEmail(string content)
        {
            return string.Format(HtmlTemplate, content);
        }

        public static string ComposeTheEmail(string content)
        {
            return string.Format(HtmlTemplate, content);
        }

    }


    public static class EmailConstants
    {
        public const string NoReplyEmail = "noreply@homxly.com";
    }
}

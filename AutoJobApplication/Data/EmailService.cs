using AutoJobApplication.Interfaces;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace AutoJobApplication.Data
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string coverLetter, string subject, string body, byte[] attachment)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("AutoJobApplication", "noreply@autojobapp.com"));
            email.To.Add(new MailboxAddress("", toEmail)); // Provide the display name (can be left empty) and the email address
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                TextBody = body.Replace("[CompanyName]", "Your Company Name")
            };

            // Add attachment to the email
            builder.Attachments.Add("UpdatedCV.docx", attachment, ContentType.Parse("application/vnd.openxmlformats-officedocument.wordprocessingml.document"));

            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient(); // Fully qualify the SmtpClient
            await smtp.ConnectAsync("smtp.example.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("user@example.com", "password");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}

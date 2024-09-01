namespace AutoJobApplication.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string coverLetter, string subject, string body, byte[] attachment);
    }
}

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace ASPNetCoreMVCProject.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthSender> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthSender Options { get; } //Environment Variables

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("MVCProject@email.com", "Nicky Chau"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}

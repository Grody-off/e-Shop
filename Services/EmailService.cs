using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace e_Shop.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress($"Customer {email}", email));
            emailMessage.To.Add(new MailboxAddress("", "UR EMAIL"));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("UR EMAIL", "UR GOOGLE CODE");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}

using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WebGym.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {

            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Gym Admin", "reset.gym@mail.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };


            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.mail.ru", 465, true);
                    await client.AuthenticateAsync("reset.gym@mail.ru", "kPrk5actB2s4RevjQGRP");
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }
            }
            catch (MailKit.Security.AuthenticationException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (MailKit.Net.Smtp.SmtpCommandException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
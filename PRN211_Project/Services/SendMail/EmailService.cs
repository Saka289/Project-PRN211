using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace PRN211_Project.Services.SendMail
{
    public class EmailService : IEmailService
    {
        public void SendEmail(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("sakanatsuma289@gmail.com"));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("sakanatsuma289@gmail.com", "gjervasogcknxqlv");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}

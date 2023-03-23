using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using PRN211_Project.Models;
using MailKit.Net.Smtp;

namespace PRN211_Project.Services.SendMail
{
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
    }
}

using System.Net;
using System.Net.Mail;

namespace Proyecto_SkyInit.Services
{
    public class GmailSenderService
    {
        private readonly IConfiguration _config;

        public GmailSenderService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string destinatario, string asunto, string cuerpo)
        {
            var fromEmail = _config["SMTP_EMAIL"];
            var fromPassword = _config["SMTP_PASSWORD"];

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true,
            };

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, "SkyInit"),
                Subject = asunto,
                Body = cuerpo,
                IsBodyHtml = false
            };
            mail.To.Add(destinatario);

            smtpClient.Send(mail);
        }
    }
}
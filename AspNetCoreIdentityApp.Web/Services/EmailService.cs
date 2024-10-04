
using AspNetCoreIdentityApp.Core.OptionsModel;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AspNetCoreIdentityApp.Web.Services
{
    public class EmailService : IEmailService
    {

        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }
        public async Task SendResetPasswordEmail(string resetEmailLink, string toEmail)
        {
            var smtpClient = new SmtpClient();

            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Host = _emailSettings.Host;
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);  

            var mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(_emailSettings.Email,"gorkemkayas.com");
            mailMessage.To.Add(toEmail);

            mailMessage.Subject = "gorkemkayas.com | Reset Password";
            mailMessage.Body = @$"
                                       <h4>Reset password | Identity Project </h4>
                                       <p><a href='{resetEmailLink}'>Click the link to reset password.</a></p>";

            mailMessage.IsBodyHtml = true;

            await smtpClient.SendMailAsync(mailMessage);


        }
    }
}

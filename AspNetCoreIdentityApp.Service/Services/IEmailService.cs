namespace AspNetCoreIdentityApp.Service.Services
{
    public interface IEmailService
    {
        Task SendResetPasswordEmail(string resetEmailLink, string toEmail);
    }
}

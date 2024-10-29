using SendingEmailWithASPNETCore.Configuration;

namespace SendingEmailWithASPNETCore.Services
{
    public interface ICustomMailService
    {
        bool SendMail(MailData Mail_Data);
        Task<bool> SendMailAsync(MailData Mail_Data);
    }
}

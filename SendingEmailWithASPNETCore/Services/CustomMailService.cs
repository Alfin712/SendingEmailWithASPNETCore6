using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using SendingEmailWithASPNETCore.Configuration;

namespace SendingEmailWithASPNETCore.Services
{
    public class CustomMailService : ICustomMailService
    {
        private readonly MailSetting Mail_Setting;

        public CustomMailService(IOptions<MailSetting> options)
        {
            Mail_Setting = options.Value ?? throw new ArgumentNullException(nameof(options), "Mail settings cannot be null.");

            // Log the values for debugging
            if (string.IsNullOrWhiteSpace(Mail_Setting.EmailId))
            {
                Log("EmailId is null or empty");
            }
            else
            {
                Log($"MailSetting - EmailId: {Mail_Setting.EmailId}, Name: {Mail_Setting.Name}");
            }
        }

        public bool SendMail(MailData Mail_Data)
        {
            try
            {
                // Validate Mail_Data and Mail_Setting before proceeding
                if (Mail_Data == null)
                {
                    Log("Mail_Data cannot be null.");
                    return false;
                }

                // Check if EmailId or Name is null or empty
                if (string.IsNullOrWhiteSpace(Mail_Setting.EmailId))
                {
                    Log("Sender EmailId cannot be null or empty.");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(Mail_Setting.Name))
                {
                    Log("Sender Name cannot be null or empty.");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(Mail_Data.EmailToId))
                {
                    Log("Recipient EmailToId cannot be null or empty.");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(Mail_Data.EmailToName))
                {
                    Log("Recipient EmailToName cannot be null or empty.");
                    return false;
                }

                //MimeMessage - a class from Mimekit
                MimeMessage email_Message = new MimeMessage();
                MailboxAddress email_From = new MailboxAddress(Mail_Setting.Name.Trim(), Mail_Setting.EmailId);
                MailboxAddress email_To = new MailboxAddress(Mail_Data.EmailToName.Trim(), Mail_Data.EmailToId);
                email_Message.From.Add(email_From);
                email_Message.To.Add(email_To);
                email_Message.Subject = Mail_Data.EmailSubject;

                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.TextBody = Mail_Data.EmailBody;
                email_Message.Body = emailBodyBuilder.ToMessageBody();

                // Connect to the SMTP server
                using (var MailClient = new SmtpClient())
                {
                    MailClient.Connect(Mail_Setting.Host, Mail_Setting.Port, Mail_Setting.UseSSL);
                    MailClient.Authenticate(Mail_Setting.EmailId, Mail_Setting.Password);
                    MailClient.Send(email_Message);
                    MailClient.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Exception Details
                Log("Error: " + ex.Message);
                return false;
            }
        }

        public Task<bool> SendMailAsync(MailData mailData)
        {
            throw new NotImplementedException();
        }

        void Log(string message)
        {
            // Implement your logging mechanism (e.g., console writeline, file write)
            Console.WriteLine(message);
        }

    }
}

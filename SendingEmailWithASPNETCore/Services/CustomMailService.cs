using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using SendingEmailWithASPNETCore.Configuration;
using System;
using System.Net;
using System.Net.Mail;

namespace SendingEmailWithASPNETCore.Services
{
    public class CustomMailService : ICustomMailService
    {
        private readonly MailSetting Mail_Setting;

        public CustomMailService(IOptions<MailSetting> options)
        {
            Mail_Setting = options.Value ?? throw new ArgumentNullException(nameof(options), "Mail settings cannot be null.");

            // Debug log untuk memeriksa nilai yang diterima
            Console.WriteLine($"Debug MailSetting - EmailId: {Mail_Setting.EmailId}, Name: {Mail_Setting.Name}");
        }

        public bool SendMail(MailData Mail_Data)
        {
            try
            {
                // Validasi input
                if (Mail_Data == null || string.IsNullOrWhiteSpace(Mail_Data.EmailToId) || string.IsNullOrWhiteSpace(Mail_Data.EmailToName))
                {
                    Log("Recipient details are missing.");
                    return false;
                }

                // Cek kredensial pengirim
                if (string.IsNullOrWhiteSpace(Mail_Setting.EmailId) || string.IsNullOrWhiteSpace(Mail_Setting.UserName) || string.IsNullOrWhiteSpace(Mail_Setting.Password))
                {
                    Log("Sender EmailId or credentials are missing.");
                    return false;
                }

                MimeMessage email_Message = new MimeMessage();
                email_Message.From.Add(new MailboxAddress(Mail_Setting.Name, Mail_Setting.EmailId));
                email_Message.To.Add(new MailboxAddress(Mail_Data.EmailToName, Mail_Data.EmailToId));
                email_Message.Subject = Mail_Data.EmailSubject;

                BodyBuilder emailBodyBuilder = new BodyBuilder { TextBody = Mail_Data.EmailBody };
                email_Message.Body = emailBodyBuilder.ToMessageBody();

                using (var MailClient = new MailKit.Net.Smtp.SmtpClient())
                {
                    // Koneksi dengan STARTTLS di port 587
                    MailClient.Connect(Mail_Setting.Host, Mail_Setting.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    MailClient.Authenticate(Mail_Setting.UserName, Mail_Setting.Password); // Perhatikan penggunaan UserName

                    MailClient.Send(email_Message);
                    MailClient.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
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

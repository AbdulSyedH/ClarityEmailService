using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
namespace EmailCore
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        EmailRequestDto emailRequest;
        private readonly string _logFilePath = "email_log.txt"; // Simple file-based logging

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            emailRequest = new EmailRequestDto();
            if(_configuration!=null)
            {
                emailRequest.SmtpServer = _configuration["SmtpSettings:Server"];
                emailRequest.SmtpPort = int.Parse(_configuration["SmtpSettings:Port"]);
                emailRequest.UserName = _configuration["SmtpSettings:Username"];
                emailRequest.Password = _configuration["SmtpSettings:Password"];
                emailRequest.FromAddress = _configuration["SmtpSettings:FromAddress"];
                emailRequest.SecondsOfDelay = int.Parse(_configuration["SmtpSettings:SecondsOfDelay"]);
            }

            // Ensure log file exists
            if (!File.Exists(_logFilePath))
            {
                File.Create(_logFilePath).Close();
            }
        }

        public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string body)
        {
            // Fire and forget - don't wait for the email to send
            return await Task.Run(() => SendEmailWithRetries(recipientEmail, subject, body));
        }

        private async Task<bool> SendEmailWithRetries(string recipientEmail, string subject, string body)
        {
            int retryCount = 0;
            bool sentSuccessfully = false;

            while (retryCount < 3 && !sentSuccessfully)
            {
                try
                {
                    using (var client = new SmtpClient(emailRequest.SmtpServer, emailRequest.SmtpPort))
                    {
                        client.Credentials = new NetworkCredential(emailRequest.UserName, emailRequest.Password);
                        client.EnableSsl = true;

                        using (var mailMessage = new MailMessage(emailRequest.FromAddress, recipientEmail, subject, body))
                        {
                            await client.SendMailAsync(mailMessage);
                            sentSuccessfully = true;
                            LogEmail(recipientEmail, subject, body, DateTime.UtcNow, "Success");
                        }
                    }
                }
                catch (Exception ex)
                {
                    retryCount++;
                    LogEmail(recipientEmail, subject, body, DateTime.UtcNow, $"Failed (Attempt {retryCount}): {ex.Message}");
                    if (retryCount < 3)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(emailRequest.SecondsOfDelay)); // Wait before retrying
                    }
                }
            }

            if (!sentSuccessfully)
            {
                LogEmail(recipientEmail, subject, body, DateTime.UtcNow, "Failed after 3 retries");
            }
            return sentSuccessfully;
        }

        private void LogEmail(string recipientEmail, string subject, string body, DateTime sentDate, string status)
        {
            string logEntry = $"Date: {sentDate:yyyy-MM-dd HH:mm:ss UTC}, To: {recipientEmail}, Subject: {subject}, Body: {body}, Status: {status}";
            File.AppendAllTextAsync(_logFilePath, logEntry + Environment.NewLine);
        }
    }
}

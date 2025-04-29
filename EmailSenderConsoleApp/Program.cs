using EmailCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EmailSenderConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var currentDiretory = Directory.GetCurrentDirectory();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var emailSender = new EmailService(configuration);

            Console.WriteLine("Enter recipient email address:");
            string? recipientEmail = Console.ReadLine();

            if (!string.IsNullOrEmpty(recipientEmail))
            {
                Console.WriteLine("Sending test email...");
                var sendEmailVal = await emailSender.SendEmailAsync(recipientEmail, "Test Email from Console App", "This is a test email sent from the console application.");
                if (sendEmailVal)
                {
                    Console.WriteLine("Email sending initiated (check email_log.txt for status).");
                }
                Console.WriteLine("Email Not sent (check email_log.txt for status)");
            }
            else
            {
                Console.WriteLine("Recipient email cannot be empty.");
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
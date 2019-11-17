﻿using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using static System.Environment;

namespace SendEmailCsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Execute().Wait();
        }

        static async Task Execute()
        {
            // var apiKey = Environment.GetEnvironmentVariable("SENDGRID_KEY_API");
            String apiKey = "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";
            Console.WriteLine("SENDGRID_API_KEY: " + apiKey);
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("workplace-futurists@hotmail.com", "Workplace Futurists");
            var tos = new List<EmailAddress>
            {
                new EmailAddress("workplace-futurists@hotmail.com", "Hotmail"),
                new EmailAddress("seungwook.l95@gmail.com", "Gmail"),
                new EmailAddress("tmdenddl@hanmail.net", "Hanmail")
            };
            var subject = "Sending Twilio SendGrid";
            var plainTextContent = "Testing";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var showAllRecipients = true; // Set to true if you want the recipients to see each others email addresses

            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from,
                                                                       tos,
                                                                       subject,
                                                                       plainTextContent,
                                                                       htmlContent,
                                                                       showAllRecipients
                                                                       );
            var response = await client.SendEmailAsync(msg);
        }

    }
}


// using System;

// namespace SendEmailCsharp
// {
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             ExecuteManualAttachmentAdd().Wait();
//             ExecuteStreamAttachmentAdd().Wait();
//         }

//         static async Task ExecuteManualAttachmentAdd()
//         {
//             var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
//             Console.WriteLine("SENDGRID_API_KEY: " + apiKey);
//             var client = new SendGridClient(apiKey);

//             var from = new EmailAddress("workplace-futurists@hotmail.com");
//             var subject = "Subject";
//             var to = new EmailAddress("seungwook.l95@gmail.com");
//             var body = "Email Body";
//             var msg = MailHelper.CreateSingleEmail(from, to, subject, body, "");
//             var bytes = File.ReadAllBytes("file.txt");
//             var file = Convert.ToBase64String(bytes);
//             msg.AddAttachment("file.txt", file);
//             var response = await client.SendEmailAsync(msg);
//         }

//         static async Task ExecuteStreamAttachmentAdd()
//         {
//             var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
//             Console.WriteLine("SENDGRID_API_KEY: " + apiKey);
//             var client = new SendGridClient(apiKey);

//             var from = new EmailAddress("workplace-futurists@hotmail.com");
//             var subject = "Subject";
//             var to = new EmailAddress("seungwook.l95@gmail.com");
//             var body = "Email Body";
//             var msg = MailHelper.CreateSingleEmail(from, to, subject, body, "");

//             using (var fileStream = File.OpenRead("C:\\Users\\username\\file.txt"))
//             {
//                 await msg.AddAttachmentAsync("file.txt", fileStream);
//                 var response = await client.SendEmailAsync(msg);
//             }
//         }

//     }
// }

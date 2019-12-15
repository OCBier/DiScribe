﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DiScribe.DatabaseManager;
using DiScribe.DatabaseManager.Data;
using DiScribe.DiScribeDebug;
using DiScribe.Email;
using DiScribe.Meeting;
using Microsoft.ProjectOxford.SpeakerRecognition;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DiScribe.DiScribeDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Execute().Wait();
            

            








            //var meeting = new DatabaseManager.Data.Meeting(0, "somesubject", "minutessdsadasd", DateTime.Now, DateTime.Now.AddMinutes(30), "", "");

            //var testEmails = new List<SendGrid.Helpers.Mail.EmailAddress>() { new SendGrid.Helpers.Mail.EmailAddress("oloff8@hotmail.com") };

            //var meetingInfo = new MeetingInfo(meeting, testEmails, "", null);




            //EmailSender.SendMinutes(meetingInfo, new FileInfo("test.txt"));

            //Console.WriteLine("testing send...");

            //var email = new SendGrid.Helpers.Mail.EmailAddress("oloff8@hotmail.com");

            //EmailSender.SendEmail(email, "something", "hiii", null);




        }


        static async Task Execute()
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("discribe_test@outlook.com", "Example User");


            var subject = "Did it work?";

            var to = new EmailAddress("kengqiangmk@gmail.com", "Example User");

            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);

            var responseBody = await response.DeserializeResponseBodyAsync(response.Body);

            if (responseBody != null)
            {
                foreach (var elem in responseBody)
                {
                    Console.WriteLine(elem.Value);

                }
            }
            
            var responseHeaders = response.DeserializeResponseHeaders(response.Headers);


            Console.WriteLine("\n\nHEADERS:");

            foreach (var header in responseHeaders)
            {
                Console.WriteLine(header.Value);

            }

            Console.WriteLine("\n\nRESPONSE CODE: " + response.StatusCode);

        }





    }
}

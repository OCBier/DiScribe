﻿using System;
using System.Collections.Generic;
using SendGrid.Helpers.Mail;

namespace MeetingControllers
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> names = new List<string>();
            names.Add("Kevin");
            names.Add("Workplace-futurists");

            foreach (string name in names)
            {
                Console.WriteLine("name: " + name);
            }
            

            List<string> emails = new List<string>();
            emails.Add("seungwook.l95@gmail.com");
            emails.Add("workplace-futurists@hotmail.com");

            foreach (string email in emails)
            {
                Console.WriteLine("email: " + email);
            }

            string startDate = "11/26/2019 15:00:00";
            string duration = "30";

            string accessCode = MeetingController.CreateWebExMeeting(names, emails, startDate, duration);
            Console.WriteLine("AccessCode: " + accessCode);
        }
    }
}

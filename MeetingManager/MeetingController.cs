﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using DiScribe.Email;
using EmailAddress = SendGrid.Helpers.Mail.EmailAddress;
using DiScribe.DatabaseManager;
using System.Text.RegularExpressions;

namespace DiScribe.Meeting
{
    public static class MeetingController
    {
        public static string BOT_EMAIL;

        public static string CreateWebExMeeting(string meetingSubject, List<string> names, List<string> emails, string startDate, string duration, WebexHostInfo hostInfo)
        {
            string strXMLServer = "https://companykm.my.webex.com/WBXService/XMLService";

            WebRequest request = WebRequest.Create(strXMLServer);
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";

            // Create POST data and convert it to a byte array.
            // string strXML = GenerateXMLCreateMeeting();

            // string strXML = File.ReadAllText(@"createMeeting.xml");
            string strXML = XMLHelper.GenerateMeetingXML(meetingSubject, names, emails, startDate, duration, hostInfo);

            byte[] byteArray = Encoding.UTF8.GetBytes(strXML);

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();

            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            string accessCode = XMLHelper.RetrieveAccessCode(responseFromServer);

            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            Console.WriteLine("\tMeeting has been successfully created");
            return accessCode;
        }

        /* This function sends email to all unregistered users
         * given all the meeting attendees
         */
        public static void SendEmailsToAnyUnregisteredUsers(List<EmailAddress> attendees, 
            string dbConnectionString = 
            "Server=tcp:dbcs319discribe.database.windows.net, 1433; " +
            "Initial Catalog=db_cs319_discribe; " +
            "Persist Security Info=False;User ID=obiermann; " +
            "Password=JKm3rQ~t9sBiemann; " +
            "MultipleActiveResultSets=True; " +
            "Encrypt=True;TrustServerCertificate=False; " +
            "Connection Timeout=30")
        {
            try
            {
                DatabaseController.Initialize(dbConnectionString);

                var unregistered = DatabaseController.GetUnregisteredUsersFrom(
                    EmailHelper.FromEmailAddressListToStringList(attendees));

                EmailSender.SendEmailForVoiceRegistration(
                    EmailHelper.FromStringListToEmailAddressList(unregistered));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public static List<EmailAddress> GetAttendeeEmails(string accessCode, WebexHostInfo meetingInfo)
        {
            Console.WriteLine(">\tRetrieving All Attendees' Emails...");
            string strXMLServer = "https://companykm.my.webex.com/WBXService/XMLService";

            WebRequest request = WebRequest.Create(strXMLServer);
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";

            // Create POST data and convert it to a byte array.
            string strXML = XMLHelper.GenerateXML(accessCode, meetingInfo);
                                    
            byte[] byteArray = Encoding.UTF8.GetBytes(strXML);

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;



            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.



            WebResponse response = request.GetResponse();

            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseStr = reader.ReadToEnd();
            // Display the content.

         
            List<EmailAddress> emailAddresses = GetEmails(responseStr);

          
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            return emailAddresses;
        }

        
        [ObsoleteAttribute("This method is depricated and does not work in all cases.")]
        public static DateTime GetMeetingTimeByXML(string accessCode, WebexHostInfo meetingInfo)
        {
            Console.WriteLine(">\tRetrieving Meeting Info...");
            string strXMLServer = "https://companykm.my.webex.com/WBXService/XMLService";

            WebRequest request = WebRequest.Create(strXMLServer);
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";

            // Create POST data and convert it to a byte array.
            string strXML = XMLHelper.GenerateInfoXML(accessCode, meetingInfo);

            byte[] byteArray = Encoding.UTF8.GetBytes(strXML);

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();

            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();

            // Display the content.
            string startDate = XMLHelper.RetrieveStartDate(responseFromServer);

            //Time zone format is like ABC-H:MM, Common (Specific). Just take zone code.
            string timeZone = XMLHelper.RetrieveTimeZone(responseFromServer).Split(" ")[0];
            timeZone = Regex.Split(timeZone, @"-?\d+")[0];

            Console.WriteLine("startTime: " + startDate);
            Console.WriteLine("timeZone: " + timeZone);

            DateTime meetingTime;
            DateTime.TryParse($"{startDate}", out meetingTime);

            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            return meetingTime;
        }

        private static List<EmailAddress> GetEmails(string myXML)
        {
            var emails = RetrieveEmails(myXML);
            var names = RetrieveNames(myXML);
            List<EmailAddress> emailAddresses = new List<EmailAddress>();

            for (int i = 0; i < emails.Count; i++)
            {
                if (emails[i].Equals(BOT_EMAIL, StringComparison.CurrentCultureIgnoreCase))
                    continue;
                Console.WriteLine("\t-\t" + emails[i]);
                emailAddresses.Add(new EmailAddress(emails[i], names[i]));
            }

            return emailAddresses;
        }

        private static List<string> RetrieveEmails(string myXML)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(myXML);

            XmlNodeList emailNodes = xml.GetElementsByTagName("com:email");

            List<string> emails = new List<string>();

            foreach (XmlNode emailNode in emailNodes)
            {
                emails.Add(emailNode.InnerText);
            }

            return emails;
        }

        private static List<string> RetrieveNames(string myXML)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(myXML);

            XmlNodeList nameNodes = xml.GetElementsByTagName("com:name");

            List<string> names = new List<string>();

            foreach (XmlNode nameNode in nameNodes)
            {
                names.Add(nameNode.InnerText);
            }

            return names;
        }
    }
}

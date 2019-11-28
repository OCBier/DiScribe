﻿using System;
using System.Collections.Generic;
using DiScribe.Transcriber;
using DiScribe.DatabaseManager;
using DiScribe.DatabaseManager.Data;
using SendGrid.Helpers.Mail;
using System.IO;
using DiScribe.Dialer;
using DiScribe.MeetingManager;
using Microsoft.Extensions.Configuration;
using DiScribe.Scheduler;
using System.Threading.Tasks;

namespace DiScribe.Main
{
    public static class Program
    {
        static async void Main(string[] args)
        {

            const bool RELEASE = true;
            /*Main application loop */
            while (true)
            {
                await ListenForInvitations(RELEASE);
            }
        }     
        



        public static async void Run(string accessCode, bool release = false)
        {
            // Set Authentication configurations
            var appConfig = Configurations.LoadAppSettings();

            // dialing & recording
            var rid = new DialerController(appConfig).CallMeetingAsync(accessCode).Result;
            var recording = new RecordingController(appConfig).DownloadRecordingAsync(rid, release).Result;

            // retrieving all attendees' emails as a List
            List<EmailAddress> invitedUsers = MeetingController.GetAttendeeEmails(accessCode);
            
            // Make controller for accessing registered user profiles in Azure Speaker Recognition endpoint
            var regController = RegistrationController.BuildController(
                EmailController.FromEmailAddressListToStringList(invitedUsers));

            // initializing the transcribe controller 
            var transcribeController = new TranscribeController(recording, regController.UserProfiles);

            // performs transcription and speaker recognition
            if (transcribeController.Perform())
                EmailController.SendMinutes(invitedUsers, transcribeController.WriteTranscriptionFile(rid, release));            
            else
                EmailController.SendEmail(invitedUsers, "Failed To Generate Meeting Transcription", "");

            Console.WriteLine(">\tTask Complete!");
        }



        /// <summary>
        /// Logic:
        ///     -> Every 10 seconds, read inbox
        ///     -> If there is a message, get access code from it
        ///     -> Call webex API to get meeting time from access code????
        ///     -> Schedule the rest of the dial to transcribe workflow
        ///         
        /// </summary>
        /// <param name="release"></param>
        /// <returns></returns>
        private static async Task ListenForInvitations(Boolean release)
        {
            //Read bot inbox,
            //If bot receives email, schedule the bot to dial in at the meeting time

            while (true)
            {
                Task.Delay(10000);

                var message = await GraphHelper.GetEmailAsync();
                string accessCode = await GraphHelper.GetEmailMeetingNumAsync(message);
                //DateTime meetingTime = await MeetingController.GetMeetingTime(accessCode);

                
                MeetingController.SendEmailsToAnyUnregisteredUsers(MeetingController.GetAttendeeEmails(accessCode));


                SchedulerController.Schedule(Run, accessCode, release, meetingTime);

            }



        }










        private static void LineBreak()
        {
            Console.Write("Press <return> to continue");
            Console.ReadLine();
        }






    }
}

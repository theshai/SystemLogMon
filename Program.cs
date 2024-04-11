using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;

namespace SystemLogMonitor
{
    class Program
    {
        static void Main(string[] args)
        {

            if (ReadEvenLog())
            {
                 string msg = "SELECT  * FROM[edihqdb].[dbo].[edi_event_detail]  where timestamp< '2023-10-01'" + "/n" +
                                "          DELETE FROM[edihqdb].[dbo].[edi_event_detail] where timestamp< '2023-10-01";
                 sendEmailNotify("EDI SQL Error",msg);

            }
           
        }

        static bool ReadEvenLog()
        {
            string eventLogName = "Application";
            string sourceName = "EventLoggingApp";
            string machineName = "edias2";
            bool retVal = false;

            EventLog eventLog = new EventLog();
            eventLog.Log = eventLogName;
            eventLog.Source = sourceName;
            eventLog.MachineName = machineName;
            

            foreach (EventLogEntry log in eventLog.Entries)
            {
                if (log.Source == "MSSQL$SQLEXPRESS")
                {//MSSQL$SQLEXPRESS
                    Console.WriteLine("{0}\n", log.Source);
                    return true;
                    //send an emaill
                }
            }

            return retVal;
        }

        static void sendEmailNotify(string subjuct, string msg)
        {
            string smtpAddress = ConfigurationSettings.AppSettings["smtpIP"];
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient();
            mail.To.Add("shai@autoland.ca");
            mail.From = new MailAddress("EDIMonitoring@autoland.ca");
            mail.Subject = subjuct;
            mail.IsBodyHtml = true;
            mail.Body = msg;
            SmtpServer.Host = smtpAddress;
            SmtpServer.Port = 25;
            SmtpServer.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            try
            {
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception Message: " + ex.Message);
                if (ex.InnerException != null)
                    Debug.WriteLine("Exception Inner:   " + ex.InnerException);
            }

        }

    }


}

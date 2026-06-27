using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for Logger
/// </summary>
public class Logger
{
    public static void Log(Exception exception)
    {
        Log(exception, EventLogEntryType.Error);
    }

    public static void Log(Exception exception, EventLogEntryType eventLogEnteryType)
    {
        StringBuilder sbExceptionMessage = new StringBuilder();
        do
        {
            sbExceptionMessage.Append("Log Date & Time " + DateTime.Now + Environment.NewLine);
            sbExceptionMessage.Append("Exception Type " + Environment.NewLine);
            sbExceptionMessage.Append(exception.GetType().Name);
            sbExceptionMessage.Append("Message" + Environment.NewLine);
            sbExceptionMessage.Append(exception.Message + Environment.NewLine);
            sbExceptionMessage.Append("Stack Trace" + Environment.NewLine);
            sbExceptionMessage.Append(exception.StackTrace + Environment.NewLine);
            sbExceptionMessage.Append("------------------------------------------------------------------------------------------------------" + Environment.NewLine);
            exception = exception.InnerException;
        } while (exception != null);

        string logProvider = ConfigurationManager.AppSettings["LogProvider"];

        if (logProvider.ToLower() == "database")
        {
            LogToDB(sbExceptionMessage.ToString());
        }
        //else if (logProvider.ToLower() == "eventViewer")
        //{
        //    LogToEventViewer(sbExceptionMessage.ToString());
        //}
        //else if (logProvider.ToLower() == "both")
        //{
        //    LogToDB(sbExceptionMessage.ToString());
        //    LogToEventViewer(sbExceptionMessage.ToString());
        //}
        //string sendEmail = ConfigurationManager.AppSettings["SendEmail"];
        //if (sendEmail.ToLower() == "true")
        //{
        //    SendEmail(sbExceptionMessage.ToString());
        //}
    }

    private static void LogToEventViewer(string log)
    {
        //if (EventLog.SourceExists("PragimTech.com"))
        //{
        //    EventLog log = new EventLog("PragimTech");
        //    log.Source = "PragimTech.com";

        //    log.WriteEntry(sbExceptionMessage.ToString(), EventLogEntryType.Error)
        //}
        string path = HttpContext.Current.Server.MapPath("~/Log/Log.txt");
        if (File.Exists(path))
        {
            using (StreamWriter wr = File.AppendText(path))
            {
                wr.WriteLine(log);
                wr.Flush();
                wr.Close();
            }
        }
    }

    private static void LogToDB(string log)
    {
        string cs = ConfigurationManager.ConnectionStrings["nagar_nigam"].ConnectionString;
        SqlConnection con = new SqlConnection(cs);
        using (SqlCommand cmd = new SqlCommand("spInsertLog", con))
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            SqlParameter param = new SqlParameter("@ExceptionMessage", log);
            cmd.Parameters.Add(param);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }

    public static void SendEmail(string emailBody)
    {
        MailMessage mailMessage = new MailMessage("mismisspl@gmail.com", "gyanchandverma.cse@gmail.com");
        mailMessage.Subject = "Exception";
        mailMessage.Body = emailBody;

        SmtpClient smtpClient = new SmtpClient();
        //SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
        //smtpClient.Credentials = new System.Net.NetworkCredential()
        //{
        //    UserName = "mismisspl@gmail.com",
        //    Password = "Gyan_1992"
        //};
        //smtpClient.EnableSsl = true;
        try
        {
            smtpClient.Send(mailMessage);
        }
        catch (Exception ex)
        {
            string msg = ex.Message;
        }
    }
}
using System;
using System.Net;
using System.Net.Mail;

using UnityEngine;

public class MailingService 
{
    // create a new MailMessage
    private readonly MailMessage _mail = new();
    

    public void SendEmail(string recipient, string subject, string body, string cords)
    {
        try
        {
            // email setup 
            _mail.From = new MailAddress("karam.mannai@esprit.tn");
            _mail.To.Add(recipient);
            _mail.Subject = subject;
            if (cords == null) _mail.Body = body;else _mail.Body = body + cords;
        
            // smtp setup
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new NetworkCredential("karam.mannai@esprit.tn", "hmfp itbl agwh xrvx");
            smtpServer.EnableSsl = true;
        
            // security certificate
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, errors) => true;
        
            // send email
            smtpServer.SendMailAsync(_mail);
            
            // log
            Debug.Log("Email sent successfully !");
        }
        catch (Exception e)
        {
            // error log
            Debug.Log(e);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Payroll.Models.Utilerias
{
    public class Mail
    {

        public Boolean SendMail(string subject, string type, string body, int flagCopy)
        {
            Boolean flag = false;
            SmtpClient client = new SmtpClient("smtp.office365.com", 587);
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("ipsnetnotificaciones@raciti.com.mx", "YA0@5faD+pbPasV^r7jS");
            MailAddress from = new MailAddress("ipsnetnotificaciones@raciti.com.mx", String.Empty, System.Text.Encoding.UTF8);
            MailAddress to = new MailAddress("marcoc@raciti.com.mx", "DeveloperSenior");
            MailMessage message = new MailMessage(from, to);
            if (flagCopy == 1) {
                MailAddress copy = new MailAddress("sergiov@raciti.com.mx", "PM");
                message.CC.Add(copy);
            }
            try {
                message.Body = "Este es un mensaje de prueba";
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = "Pruebas";
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.Priority = MailPriority.High;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                client.Send(message);
                flag = true;
            } catch (Exception exc) {
                Console.WriteLine(exc.Message.ToString());
            } finally {
                client.Dispose();
                message.Dispose();
            }
            return flag;
        }


    }
}
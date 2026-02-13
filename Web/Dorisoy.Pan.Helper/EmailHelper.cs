using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Dorisoy.Pan.Helper
{
    public class EmailHelper
    {
        static List<MemoryStream> attachments = new List<MemoryStream>();
        public static void SendEmail(SendEmailSpecification sendEmailSpecification)
        {
            MailMessage message = new MailMessage()
            {
                Sender = new MailAddress(sendEmailSpecification.FromAddress),
                From = new MailAddress(sendEmailSpecification.FromAddress),
                Subject = sendEmailSpecification.Subject,
                IsBodyHtml = true,
                Body = sendEmailSpecification.Body,
            };

            if (sendEmailSpecification.Attechments.Count > 0)
            {
                Attachment attach;
                foreach (var file in sendEmailSpecification.Attechments)
                {

                    string fileData = file.Src.Split(',').LastOrDefault();
                    byte[] bytes = Convert.FromBase64String(fileData);
                    var ms = new MemoryStream(bytes);
                    attach = new Attachment(ms, file.Name, file.FileType);
                    attachments.Add(ms);
                    message.Attachments.Add(attach);
                }
            }
            sendEmailSpecification.ToAddress.Split(",").ToList().ForEach(toAddress =>
            {
                message.To.Add(new MailAddress(toAddress));
            });
            if (!string.IsNullOrEmpty(sendEmailSpecification.CCAddress))
            {
                sendEmailSpecification.CCAddress.Split(",").ToList().ForEach(ccAddress =>
                {
                    message.CC.Add(new MailAddress(ccAddress));
                });
            }

            SmtpClient smtp = new SmtpClient()
            {
                Port = sendEmailSpecification.Port,
                Host = sendEmailSpecification.Host,
                EnableSsl = sendEmailSpecification.IsEnableSSL,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(sendEmailSpecification.UserName, sendEmailSpecification.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            smtp.Send(message);
            if (attachments.Count() > 0)
            {
                foreach (var attachment in attachments)
                {
                    try
                    {
                        attachment.Dispose();
                    }
                    catch
                    {
                    }
                }
            }

        }

        public static void SendFileOrFolder(SendEmailSpecification sendEmailSpecification)
        {
            MailMessage message = new MailMessage()
            {
                Sender = new MailAddress(sendEmailSpecification.FromAddress),
                From = new MailAddress(sendEmailSpecification.FromAddress),
                Subject = sendEmailSpecification.Subject,
                IsBodyHtml = true,
                Body = sendEmailSpecification.Body,
            };

            if (sendEmailSpecification.Attachment != null)
            {
                message.Attachments.Add(sendEmailSpecification.Attachment);
            }
            sendEmailSpecification.ToAddress.Split(",").ToList().ForEach(toAddress =>
            {
                message.To.Add(new MailAddress(toAddress));
            });
            if (!string.IsNullOrEmpty(sendEmailSpecification.CCAddress))
            {
                sendEmailSpecification.CCAddress.Split(",").ToList().ForEach(ccAddress =>
                {
                    message.CC.Add(new MailAddress(ccAddress));
                });
            }

            SmtpClient smtp = new SmtpClient()
            {
                Port = sendEmailSpecification.Port,
                Host = sendEmailSpecification.Host,
                EnableSsl = sendEmailSpecification.IsEnableSSL,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(sendEmailSpecification.UserName, sendEmailSpecification.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            smtp.Send(message);
        }
    }


}

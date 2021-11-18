using Ecommerce.Dto;
using Ecommerce.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ecommerce.Util
{
    public class DataHelper
    {
        public static string SHA256Hash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string Unsign(string value)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = value.Normalize(NormalizationForm.FormD);
            string str1 = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            return Regex.Replace(str1, @"[^0-9a-zA-Z]+", "-").ToLower();
        }

        public static string ToCurrency(double? value)
        {
            return string.Format("{0:#,##0}", value).Replace(",", ".");
        }

        public static void SendMail(EmailConfiguration emailConfig, string subject, string body, List<string> to, List<string> cc = null, List<string> bcc = null)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com");
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(emailConfig.Email, "DEcommerce Notification");
            mail.Subject = subject;
            //mail.Body = body;
            mail.IsBodyHtml = true;
            AlternateView alterView = ContentToAlternateView(body);
            mail.AlternateViews.Add(alterView);

            if (to != null && to.Count > 0)
            {
                foreach (string email in to)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                        mail.To.Add(new MailAddress(email.Trim()));
                }
            }

            if (cc != null && cc.Count > 0)
            {
                foreach (string email in cc)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                        mail.CC.Add(new MailAddress(email.Trim()));
                }
            }

            if (bcc != null && bcc.Count > 0)
            {
                foreach (string email in bcc)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                        mail.Bcc.Add(new MailAddress(email.Trim()));
                }
            }

            client.Port = 587;
            client.Credentials = new System.Net.NetworkCredential(emailConfig.Email, emailConfig.Password);
            client.EnableSsl = true;
            client.Send(mail);
        }
        private static AlternateView ContentToAlternateView(string content)
        {
            var imgCount = 0;
            List<LinkedResource> resourceCollection = new List<LinkedResource>();
            foreach (Match m in Regex.Matches(content, "<img(?<value>.*?)>"))
            {
                imgCount++;
                var imgContent = m.Groups["value"].Value;
                string type = Regex.Match(imgContent, ":(?<type>.*?);base64,").Groups["type"].Value;
                string base64 = Regex.Match(imgContent, "base64,(?<base64>.*?)\"").Groups["base64"].Value;
                if (String.IsNullOrEmpty(type) || String.IsNullOrEmpty(base64))
                {
                    //ignore replacement when match normal <img> tag
                    continue;
                }
                var replacement = " src=\"cid:" + imgCount + "\"";
                content = content.Replace(imgContent, replacement);
                var tempResource = new LinkedResource(Base64ToImageStream(base64), new ContentType(type))
                {
                    ContentId = imgCount.ToString()
                };
                resourceCollection.Add(tempResource);
            }

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(content, null, MediaTypeNames.Text.Html);
            foreach (var item in resourceCollection)
            {
                alternateView.LinkedResources.Add(item);
            }

            return alternateView;
        }

        public static Stream Base64ToImageStream(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            return ms;
        }
    }
}

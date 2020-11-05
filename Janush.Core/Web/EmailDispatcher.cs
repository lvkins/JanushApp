using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Janush.Core
{
    public class EmailDispatcher : IEmailDispatcher
    {
        /// <summary>
        /// Sends a given content to the configured email address.
        /// </summary>
        /// <param name="subject">The message subject.</param>
        /// <param name="body">The message content.</param>
        /// <param name="isHtml">If the message body markup is HTML.</param>
        /// <returns><see cref="true"/> if message was sent, otherwise <see cref="false"/>.</returns>
        public async Task<bool> SendMessage(string subject, string body, bool isHtml = true)
        {
            var settings = CoreDI.SettingsReader.Settings;

            // Read user settings
            if (!settings.EmailNotifications)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(settings.EmailHost)
            || string.IsNullOrWhiteSpace(settings.EmailUsername)
            || !int.TryParse(settings.EmailPort, out var port))
            {
                return false;
            }

            try
            {
                using (var client = new SmtpClient(settings.EmailHost, port)
                {
                    // Set SMTP-client with basicAuthentication
                    UseDefaultCredentials = false,
                    EnableSsl = settings.EmailUseTLS,
                })
                {

                    if (settings.EmailUseAuth)
                    {
                        client.Credentials = new NetworkCredential(settings.EmailUsername,
                            new SecureString().RNGCryptoDecrypt(settings.EmailPassword, settings.EmailPasswordHash));
                    }

                    // Configure email addresses
                    var from = new MailAddress(settings.EmailUsername);
                    var to = new MailAddress(settings.EmailUsername);
                    var mail = new MailMessage(from, to)
                    {
                        // set subject and encoding
                        Subject = $"({Consts.APP_TITLE}) {subject}",
                        SubjectEncoding = Encoding.UTF8,

                        // set body-message and encoding
                        Body = body,
                        BodyEncoding = Encoding.UTF8,

                        // text or HTML
                        IsBodyHtml = isHtml
                    };

                    await client.SendMailAsync(mail);

                    return true;
                }
            }
            catch (SmtpException ex)
            {
                // TODO: leave log entry
                if (Debugger.IsAttached)
                {
                    throw new ApplicationException
                      ("SmtpException has occurred: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }
    }
}

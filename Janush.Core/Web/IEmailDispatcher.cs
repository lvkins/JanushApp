using System.Threading.Tasks;

namespace Janush.Core
{
    public interface IEmailDispatcher
    {
        /// <summary>
        /// Sends a given content to the configured email address.
        /// </summary>
        /// <param name="subject">The message subject.</param>
        /// <param name="body">The message content.</param>
        /// <param name="isHtml">If the message body markup is HTML.</param>
        /// <returns><see cref="true"/> if message was sent, otherwise <see cref="false"/>.</returns>
        Task<bool> SendMessage(string subject, string body, bool isHtml = true);
    }
}

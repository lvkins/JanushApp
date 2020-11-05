using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Janush.Core
{
    /// <summary>
    /// The web configuration file.
    /// </summary>
    public class WebConfig
    {
        /// <summary>
        /// The application version.
        /// </summary>
        public int Version { get; set; }
    }

    /// <summary>
    /// A class for monitoring the application version.
    /// </summary>
    public static class VersionMonitor
    {
        /// <summary>
        /// Checks whether there is a new application version available.
        /// </summary>
        /// <returns><see cref="true"/> if a newer version is available, otherwise <see cref="false"/>.</returns>
        public static async Task<bool> IsNewAsync()
        {
            // Get assembly version
            int.TryParse(Consts.APP_VERSION.Replace(".", ""), out var assemblyVersion);
            return await GetCurrentAsync() > assemblyVersion;
        }

        /// <summary>
        /// Loads application version from the external server.
        /// </summary>
        /// <returns>The current application version.</returns>
        public static async Task<int> GetCurrentAsync()
        {
            // Create a web request
            var request = (HttpWebRequest)WebRequest.Create(Consts.APP_URL + "/config.json");

            // Do not follow redirects
            request.AllowAutoRedirect = false;

            // Get only headers
            request.Method = WebRequestMethods.Http.Get;

            // Timeout after 10 seconds
            request.Timeout = 10000;

            // Add headers
            request.Headers.Clear();
            request.Headers.Add(HttpRequestHeader.UserAgent, Consts.USER_AGENT);

            try
            {
                using (var response = await request.GetResponseAsync())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    // Deserialize response
                    var config = JsonConvert.DeserializeObject<WebConfig>(await reader.ReadToEndAsync());
                    return config.Version;
                }
            }
            catch
            {
                // Leave log message
                CoreDI.Logger.Error("Failed parsing web config");
            }

            return 0;
        }
    }
}

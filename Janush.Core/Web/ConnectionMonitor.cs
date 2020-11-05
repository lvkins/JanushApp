using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Janush.Core
{
    /// <summary>
    /// A class for monitoring the internet connection.
    /// </summary>
    public class ConnectionMonitor : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// The worker task.
        /// </summary>
        private readonly Task _task;

        /// <summary>
        /// The check interval.
        /// </summary>
        private readonly TimeSpan _interval;

        /// <summary>
        /// A callback to be invoked once state changes.
        /// </summary>
        private readonly Action<bool> _onStateChange;

        /// <summary>
        /// The worker task cancellation token.
        /// </summary>
        private readonly CancellationTokenSource _cancellationToken;

        /// <summary>
        /// Current connection state.
        /// </summary>
        private bool _state = true;

        #endregion

        #region Constructor

        public ConnectionMonitor(TimeSpan interval, Action<bool> onStateChange)
        {
            _interval = interval;
            _onStateChange = onStateChange;
            _cancellationToken = new CancellationTokenSource();
            _task = Task.Run(WorkerTaskAsync, _cancellationToken.Token);
        }

        #endregion

        #region Private Methods

        private async void WorkerTaskAsync()
        {
            while (true)
            {
                // Break once cancellation requested
                if (_cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                // Await interval delay
                await Task.Delay(_interval);

                // Get result
                var result = await HttpEndpointReachableAsync();

                Debug.WriteLine($"[Internet Connection] {result}");

                // If state has changed...
                if (result != _state)
                {
                    // Invoke callback
                    _onStateChange(result);
                }

                // Update state
                _state = result;
            }
        }

        /// <summary>
        /// Verifies whether the external server is reachable.
        /// </summary>
        /// <returns><see langword="true"/> if we are able to reach the endpoint, otherwise <see langword="false"/>.</returns>
        private async Task<bool> HttpEndpointReachableAsync()
        {
            // If card status is down, don't bother pinging
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }

            // Create a web request
            var request = (HttpWebRequest)WebRequest.Create("https://www.google.com/");

            // Do not follow redirects
            request.AllowAutoRedirect = false;

            // Get only headers
            request.Method = WebRequestMethods.Http.Head;

            // Timeout after 10 seconds
            request.Timeout = 10000;

            try
            {
                using (var response = await request.GetResponseAsync())
                {
                    // Read response status code
                    var status = ((HttpWebResponse)response).StatusCode;

                    // If status is not successful...
                    if (status != HttpStatusCode.OK && status != HttpStatusCode.Moved)
                    {
                        return false;
                    }

                    // Apparently the connection is fine
                    // Return successful result
                    return true;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            // Execute token
            using (_cancellationToken)
            {
                _cancellationToken.Cancel();
            }
        }

        #endregion
    }
}

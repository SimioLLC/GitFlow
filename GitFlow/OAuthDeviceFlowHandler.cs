using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GitFlow
{
    public class DeviceCodeResponse
    {
        public string device_code { get; set; }
        public string user_code { get; set; }
        public string verification_uri { get; set; }
        public int expires_in { get; set; }
        public int interval { get; set; }
    }

    public class OAuthTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
    }

    public class OAuthDeviceFlowHandler
    {
        // Simio LLC should register a GitHub OAuth App at https://github.com/settings/applications/new
        // and replace this placeholder with the actual client_id.
        // For development/testing, register your own OAuth App.
        private const string GitHubClientId = "REPLACE_WITH_GITHUB_OAUTH_APP_CLIENT_ID";

        public static bool IsConfigured => GitHubClientId != "REPLACE_WITH_GITHUB_OAUTH_APP_CLIENT_ID";

        private static readonly HttpClient _httpClient = new HttpClient();

        static OAuthDeviceFlowHandler()
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Initiates the GitHub OAuth Device Flow and returns a DeviceCodeResponse
        /// containing the user_code and verification_uri to show the user.
        /// </summary>
        public static async Task<DeviceCodeResponse> RequestDeviceCode()
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", GitHubClientId),
                new KeyValuePair<string, string>("scope", "repo")
            });

            var response = await _httpClient.PostAsync(
                "https://github.com/login/device/code", content);

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var deviceCode = JsonSerializer.Deserialize<DeviceCodeResponse>(json);
            if (deviceCode == null || string.IsNullOrEmpty(deviceCode.device_code))
            {
                throw new Exception("Invalid device code response from GitHub.");
            }
            return deviceCode;
        }

        /// <summary>
        /// Polls GitHub for the access token after the user has entered the device code.
        /// Returns the access token string on success, or throws on timeout/denial.
        /// </summary>
        public static async Task<string> PollForAccessToken(
            DeviceCodeResponse deviceCode,
            CancellationToken cancellationToken)
        {
            var pollInterval = Math.Max(deviceCode.interval, 5);
            var expiresAt = DateTime.UtcNow.AddSeconds(deviceCode.expires_in);

            while (DateTime.UtcNow < expiresAt)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(pollInterval * 1000, cancellationToken);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", GitHubClientId),
                    new KeyValuePair<string, string>("device_code", deviceCode.device_code),
                    new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:device_code")
                });

                var response = await _httpClient.PostAsync(
                    "https://github.com/login/oauth/access_token", content, cancellationToken);

                // Per RFC 8628, the polling endpoint can legitimately return 4xx with an
                // OAuth error code in the body (authorization_pending, slow_down, etc.),
                // so we don't call EnsureSuccessStatusCode here. We do reject 5xx responses
                // and any unparseable body to avoid acting on garbage.
                if ((int)response.StatusCode >= 500)
                {
                    throw new Exception(
                        $"GitHub OAuth server error: {(int)response.StatusCode} {response.ReasonPhrase}");
                }

                var json = await response.Content.ReadAsStringAsync();
                OAuthTokenResponse tokenResponse;
                try
                {
                    tokenResponse = JsonSerializer.Deserialize<OAuthTokenResponse>(json);
                }
                catch (JsonException ex)
                {
                    throw new Exception("Unexpected response from GitHub OAuth endpoint.", ex);
                }
                if (tokenResponse == null)
                {
                    throw new Exception("Empty response from GitHub OAuth endpoint.");
                }

                if (!string.IsNullOrEmpty(tokenResponse.access_token))
                {
                    return tokenResponse.access_token;
                }

                switch (tokenResponse.error)
                {
                    case "authorization_pending":
                        // User hasn't entered the code yet, keep polling
                        continue;
                    case "slow_down":
                        // We're polling too fast, increase interval
                        pollInterval += 5;
                        continue;
                    case "expired_token":
                        throw new Exception("The authorization code has expired. Please try again.");
                    case "access_denied":
                        throw new Exception("Authorization was denied by the user.");
                    default:
                        if (!string.IsNullOrEmpty(tokenResponse.error))
                            throw new Exception($"OAuth error: {tokenResponse.error} - {tokenResponse.error_description}");
                        continue;
                }
            }

            throw new TimeoutException("Authorization timed out. Please try again.");
        }

        /// <summary>
        /// Convenience method that runs the full GitHub OAuth Device Flow:
        /// 1. Requests a device code
        /// 2. Shows the OAuthProgressForm to the user
        /// 3. Polls for the token
        /// 4. Returns the access token
        /// </summary>
        public static async Task<string> AuthenticateGitHub()
        {
            var deviceCode = await RequestDeviceCode();

            using (var cts = new CancellationTokenSource())
            using (var progressForm = new OAuthProgressForm(deviceCode.user_code, deviceCode.verification_uri, cts))
            {
                // Start polling in the background
                var pollTask = PollForAccessToken(deviceCode, cts.Token);

                // Show the form (blocks until closed or token received)
                progressForm.SetPollTask(pollTask);
                progressForm.ShowDialog();

                if (progressForm.AccessToken != null)
                {
                    return progressForm.AccessToken;
                }

                throw new OperationCanceledException("GitHub sign-in was cancelled.");
            }
        }
    }
}

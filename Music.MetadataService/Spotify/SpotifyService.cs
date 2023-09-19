using Music.APIs.Extensions;
using Music.APIs.Spotify.Models;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Timers;
using System.Web;

namespace Music.APIs.Spotify
{
    public sealed class SpotifyService
    {
        #region Private Members

        private const int MAX_RETRY_RETRY_COUNT = 10;
        private const string TOKEN_URL = "https://accounts.spotify.com/api/token";
        private const string CLIENT_ID = "ef143bdcc8fc4d88950e2ecdab4a9b73";
        private const string CLIENT_SECRET = "d34beb6158cc4da897c066947d1fbdfb";

        private int _retryCount = 0;
        private string _accessToken = string.Empty;
        private readonly System.Timers.Timer _timer = new();
        private readonly IHttpClientFactory _httpClientFactory;

        #endregion Private Members

        public SpotifyService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            _timer.Elapsed += OnTokenExpired;
        }

        ~SpotifyService()
        {
            if (_timer is not null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }

        private async void OnTokenExpired(object? sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            _accessToken = await GetAccessTokenAsync();

            _timer.Start();
        }

        /// <summary>
        /// Creates a POST request to the token endpoint of the Spotify API.
        /// On success it starts an internal timer for token refresh.
        /// </summary>
        /// <returns>The access token from the Spotify API.</returns>
        /// <exception cref="InvalidOperationException">If the maximum number of retries is succeded.</exception>
        private async Task<string> GetAccessTokenAsync()
        {
            var authenticationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", CLIENT_ID, CLIENT_SECRET)));

            var client = _httpClientFactory.CreateClient("AuthorizationClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authenticationHeader);

            var requestBody = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };

            var content = new FormUrlEncodedContent(requestBody);

            using var response = await client.PostAsync(TOKEN_URL, content).ConfigureAwait(false);

            var json = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<SpotifyToken>(json);

            if (token is not null && token.ExpiresIn > 0)
            {
                _timer.Interval = token.ExpiresIn * 1000;
                _timer.Start();
            }
            else
            {
                if (_retryCount != MAX_RETRY_RETRY_COUNT)
                {
                    // Retry recursively
                    _retryCount++;
                    return await GetAccessTokenAsync();
                }
                else
                {
                    throw new InvalidOperationException("Max retry number of attempts reached!");
                }

            }

            return token.AccessToken;
        }

        public async Task<Tracks?> SearchTrackAsync(string track, string artist = "", int limit = 1)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("SearchClient");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                string query = BuildQuery(track, artist);

                using HttpResponseMessage response = await client.GetAsync($"https://api.spotify.com/v1/search?q={query}&type=track&limit={limit}");

                response.WriteRequestToConsole();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    // Error handling
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            Debug.WriteLine("Bad request!");
                            return null;
                        case HttpStatusCode.Forbidden:
                            Debug.WriteLine("Forbidden request!");
                            return null;
                        default:
                            break;
                    }
                }

                var content = await response.Content.ReadAsStringAsync();
                var trackModel = JsonConvert.DeserializeObject<ApiResponse>(content);

                if (trackModel is null || trackModel.Tracks is null)
                    throw new Exception("Could not deserialize json reponse.");

                return trackModel.Tracks;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private static string BuildQuery(string track, string artist)
        {
            var builder = new StringBuilder();
            builder.Append("track:");
            builder.Append(track.NormalizeString());

            if (string.IsNullOrEmpty(artist))
                return HttpUtility.UrlEncode(builder.ToString());

            builder.Append(" artist:");
            builder.Append(artist.NormalizeString());

            return HttpUtility.UrlEncode(builder.ToString());

        }
    }
    internal static class HttpResponseMessageExtensions
    {
        internal static void WriteRequestToConsole(this HttpResponseMessage response)
        {
            if (response is null)
            {
                return;
            }

            var request = response.RequestMessage;
            Debug.Write($"{request?.Method} ");
            Debug.Write($"{request?.RequestUri} ");
            Debug.WriteLine($"HTTP/{request?.Version}");
        }
    }

}

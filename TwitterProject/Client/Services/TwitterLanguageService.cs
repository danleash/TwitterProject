

namespace TwitterProject.Client.Services
{
    public class TwitterLanguageService
    {
        //*******************************************************//
        /// <summary>
        /// THIS ENTIRE ENDPOINT IS DEPRACATED THANKS TWITTER
        /// </summary>
        //*******************************************************//


        /*private readonly HttpClient _httpClient;
        private readonly ILogger<TwitterLanguageService> _logger;
        
        public TwitterLanguageService(IHttpClientFactory clientFactory, ILoggerFactory loggerFactory)
        {
            _httpClient = clientFactory.CreateClient("LanguageRequestClient");
            _logger = loggerFactory.CreateLogger<TwitterLanguageService>();
        }
        /// <summary>
        /// Gets language codes supported by twitter for our language filter.
        /// </summary>
        /// <returns>List of languages</returns>
        public async Task<List<TwitterApiLanguageResponse>> GetLanguages()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ApiKey}:{Secret}")));
                var formContent = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                //Request Access token with Apikey and Secret
                var result = await _httpClient.PostAsync("https://api.twitter.com/oauth2/token", formContent);
                var tokenResponse = JsonSerializer.Deserialize<TwitterApiOAuthResponse>(await result.Content.ReadAsStringAsync());
                if (tokenResponse == null)
                {
                    throw new Exception($"Failed to retrieve access token with credentials key: {ApiKey} secret: {Secret}");
                }

                //Set the bearer token with the access token
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
                var languageResponse = await _httpClient.GetAsync("https://api.twitter.com/1.1/help/languages.json");
                var languages = JsonSerializer.Deserialize<List<TwitterApiLanguageResponse>>(await result.Content.ReadAsStringAsync());
                return languages;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving language codes from Twitter Api");
                return null;
            }
        }*/
    }
}

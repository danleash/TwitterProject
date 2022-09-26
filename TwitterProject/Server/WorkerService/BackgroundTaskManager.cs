using System.Text;
using System.Text.Json;
using TwitterProject.Shared.Models;
using TwitterProject.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace TwitterProject.Server.WorkerService
{
    public class BackgroundTaskManager : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BackgroundTaskManager> _logger;
        private readonly IHubContext<SignalRStreamService> _signalRService;
        private Stream TwitterStreamResult { get; set; }
        private readonly TweetStorageService _tweetStorageService;

        //Horrible practice, we would use secrets or an Azure Key Vault in production - for this we're hard coding the values. 
        private readonly string ApiKey = "rkKuYxg50VKZp9BAOshvYEhNw";
        private readonly string Secret = "Oi4Q26oFfFdsxCip3kT52qFTynTI9Vqv2QJzBMqbJLGc42gICk";
        private readonly string BearerToken = "AAAAAAAAAAAAAAAAAAAAABiChQEAAAAASLJSK6KV04RZqnLMMy%2BCZHRifhk%3D7RKfe3kTnxkBXmW7HsMZH7VnAikIZy9QfIpotobEyiyliVIrAV";
        
        public BackgroundTaskManager(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, TweetStorageService tweetStorage, IHubContext<SignalRStreamService> signalRStreamService)
        {
            _httpClient = httpClientFactory.CreateClient("TwitterApiHttpClient");
            _logger = loggerFactory.CreateLogger<BackgroundTaskManager>();
            _tweetStorageService = tweetStorage;
            _signalRService = signalRStreamService;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
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
               
                await base.StartAsync(cancellationToken);
            }
            catch (HttpRequestException httpMessage)
            {
                _logger.LogError(httpMessage, $"The api request returned a non-successful result.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while starting the background service.");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                //Establish the stream from Twitter Api
                var response = await _httpClient.GetAsync("https://api.twitter.com/2/tweets/sample/stream?tweet.fields=context_annotations,lang,entities", HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                TwitterStreamResult = await response.Content.ReadAsStreamAsync();
                using var streamReader = new StreamReader(TwitterStreamResult);

                //Check condition for stopping the stream
                while (!stoppingToken.IsCancellationRequested)
                {
                    var input = await streamReader.ReadLineAsync();
                    //Check for null continue if null
                    if (string.IsNullOrWhiteSpace(input)) continue;

                    //Deserialize 
                    var streamLineData = JsonSerializer.Deserialize<TwitterStreamResponse>(input);
                    var formattedTweet = new TweetModel
                    {
                        Id = streamLineData.Data.Id,
                        Text = streamLineData.Data.Text,
                        Language = streamLineData.Data.LanguageCode,
                        TweetHeader = $"Tweet-{streamLineData.Data.Id}",
                        Entities = streamLineData.Data.Entities
                    };
                    //Check to see if a language filter exists, if it does we will only stream for the language requested. 
                    if(formattedTweet.Language == _tweetStorageService.LanguageFilter || string.IsNullOrWhiteSpace(_tweetStorageService.LanguageFilter))
                    {
                        //Increment the tweet count
                        _tweetStorageService.IncrementTweetCount(formattedTweet);

                        //Gets the live metric model to return to the client.
                        var metricModel = _tweetStorageService.ReturnLiveMetrics();

                        //Send tweets to the client
                        await _signalRService.Clients.All.SendAsync("Tweets", formattedTweet);

                        //Send the metrics to the client
                        await _signalRService.Clients.All.SendAsync("Metrics", metricModel);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error happened while reading twitter stream");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _httpClient.Dispose();
            return base.StopAsync(cancellationToken);
        }
        
    }
}

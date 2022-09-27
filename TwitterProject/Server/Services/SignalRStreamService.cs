using Microsoft.AspNetCore.SignalR;
namespace TwitterProject.Server.Services
{
    public class SignalRStreamService : Hub
    {
        private readonly TweetStorageService _storageService;
        private readonly ILogger<SignalRStreamService> _logger;
        public SignalRStreamService(TweetStorageService storageService, ILoggerFactory loggerFactory)
        {
            _storageService = storageService;
            _logger = loggerFactory.CreateLogger<SignalRStreamService>();
        }
        /// <summary>
        /// Sets the language code from an on change event on the client.
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public async Task SetLanguage (string languageCode)
        {
            _logger.LogInformation($"{languageCode} language code received on server. Applying filter to incoming streams.");
            _storageService.HashTagsPairs.Clear();
            _storageService.CurrentTweetCount = 0;
            _storageService.LanguageFilter = languageCode;
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using TwitterProject.Shared.Models;
using Microsoft.AspNetCore.ResponseCompression;
namespace TwitterProject.Server.Services
{
    public class SignalRStreamService : Hub
    {
        private string Filter { get; set; }
        private readonly TweetStorageService _storageService;

        public SignalRStreamService(TweetStorageService storageService)
        {
            _storageService = storageService;
        }
        /// <summary>
        /// Sets the language code from an on change event on the client.
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public async Task SetLanguage (string languageCode)
        {
            _storageService.HashTagsPairs.Clear();
            _storageService.CurrentTweetCount = 0;
            _storageService.LanguageFilter = languageCode;
        }
    }
}

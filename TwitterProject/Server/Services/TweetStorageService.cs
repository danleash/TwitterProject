using System.Collections.Concurrent;
using TwitterProject.Shared.Models;

namespace TwitterProject.Server.Services
{
    public class TweetStorageService
    {
        public int CurrentTweetCount { get; set; } = 0;
        public Dictionary<string, int> HashTagsPairs { get; set; } = new();
        public string? LanguageFilter { get; set; }
        private readonly ILogger _logger;
        public TweetStorageService()
        {

        }
        public TweetStorageService(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger<TweetStorageService>();
        }
        
        /// <summary>
        /// Takes in a formatted tweet from the twitter api stream and increments the total count. 
        /// </summary>
        /// <param name="model"></param>
        public void IncrementTweetCount(TweetModel model)
        {
            //Increment tweet count
            CurrentTweetCount++;
            _logger.LogInformation($"Tweet count incremented. Current tweet count is {CurrentTweetCount}");
            BuildHashTagList(model);
        }
        /// <summary>
        /// Takes in the tweet model and adds hashtags to the dictionary while incrementing those that are already existing. 
        /// </summary>
        /// <param name="model"></param>
        public void BuildHashTagList(TweetModel model)
        {
            if (model.Entities != null)
            {
                //Build the hashtag
                var hashtags = model.Entities.Hashtags;
                if (hashtags != null)
                {
                    //Iterate through each hashtag and increment count if exists or add if not existing.
                    foreach (var tag in hashtags)
                    {
                        if (HashTagsPairs != null)
                        {
                            //check if tag already added
                            if (HashTagsPairs.ContainsKey(tag.Tag))
                                HashTagsPairs[tag.Tag]++;
                            else
                            {
                                HashTagsPairs.TryAdd(tag.Tag, 1);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Orders the hash tag dictionary and takes the top 10. Builds and returns the model needed to send through SignalRConnection to the client.
        /// </summary>
        /// <returns>TweetMetricStreamModel</returns>
        public TweetMetricStreamModel ReturnLiveMetrics()
        {
            IEnumerable<KeyValuePair<string, int>> topTenHashTags;
            //While there are less than 10 tweets (common) don't limit the amount streamed to only take 10.
            if (HashTagsPairs.Count < 10)
            {
                topTenHashTags = HashTagsPairs.OrderByDescending(x => x.Value);
            }
            else
            {
                //Order to grab the most used hashtags and take top ten
                topTenHashTags = HashTagsPairs.OrderByDescending(x => x.Value).Take(10);
            }
            return new TweetMetricStreamModel
            {
                TweetCount = CurrentTweetCount,
                HashTagPairs = topTenHashTags
            };
        }
        public virtual TweetModel BuildTweet(TwitterStreamResponse streamResponse)
        {
            return new TweetModel
            {
                Id = streamResponse.Data.Id,
                Text = streamResponse.Data.Text,
                Language = streamResponse.Data.LanguageCode,
                TweetHeader = $"Tweet-{streamResponse.Data.Id}",
                Entities = streamResponse.Data.Entities
            };
        }
    }
}

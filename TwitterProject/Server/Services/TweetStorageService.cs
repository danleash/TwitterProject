using System.Collections.Concurrent;
using TwitterProject.Shared.Models;

namespace TwitterProject.Server.Services
{
    public class TweetStorageService
    {
        public int CurrentTweetCount { get; set; } = 0;
        public Dictionary<string, int> HashTagsPairs { get; set; } = new();
        public string? LanguageFilter { get; set; }

        /// <summary>
        /// Takes in a formatted tweet from the twitter api stream and increments the total count. 
        /// </summary>
        /// <param name="model"></param>
        public void IncrementTweetCount(TweetModel model)
        {
            //Increment tweet count
            CurrentTweetCount++;
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
                        if (HashTagsPairs != null && !HashTagsPairs.TryAdd(tag.Tag, 1))
                        {
                            HashTagsPairs[tag.Tag]++;
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
            if (HashTagsPairs.Count < 10) return null;
            //Order to grab the most used hashtags
            var topTenHashTags = HashTagsPairs.OrderByDescending(x => x.Value).Take(10);
            return new TweetMetricStreamModel
            {
                TweetCount = CurrentTweetCount,
                HashTagPairs = topTenHashTags
            };
        }
    }
}

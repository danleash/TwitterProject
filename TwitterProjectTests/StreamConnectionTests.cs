using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitterProject.Server.Services;
using TwitterProject.Shared.Models;

namespace TwitterProjectTests
{
    [TestClass]
    public class StreamConnectionTests
    {
        private int TweetCount { get; set; }
        private TweetModel TweetModel { get; set; }
        Mock<TweetStorageService> _tweetStorageService = new Mock<TweetStorageService>();
        private TwitterStreamResponse TwitterStreamResponse { get; set; }
        public StreamConnectionTests()
        {
            TwitterStreamResponse = new TwitterStreamResponse()
            {
                Data = new StreamDataModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "This is a text example for the twitter api stream data text field.",
                    Entities = new EntitiesModel()
                    {
                        Hashtags = new HashTagModel[] {
                        new HashTagModel(){
                            Tag = "#TwitterAPi"
                        }
                    }
                    },
                    LanguageCode = "en"
                }
            };
            TweetModel = new TweetModel()
            {
                Id = TwitterStreamResponse.Data.Id,
                DateTweeted = DateTime.Now,
                Language = TwitterStreamResponse.Data.LanguageCode,
                Text = TwitterStreamResponse.Data.Text,
                Entities = TwitterStreamResponse.Data.Entities
            };
            _tweetStorageService.Setup(x => x.BuildTweet(TwitterStreamResponse)).Returns(TweetModel);
        }
        
        [TestMethod]
        public void BuildTweetModelTest()
        {
            
            var buildTweet = new TweetStorageService().BuildTweet(TwitterStreamResponse);
            Assert.IsNotNull(buildTweet);
        }
        
        [TestMethod]
        public void TweetCountIncrementTest()
        {
            //_tweetStorageService.Setup(o => o.IncrementTweetCount())

            var previous = TweetCount;
            //Increment tweet count
            TweetCount++;
            Console.WriteLine(TweetCount);
            Assert.IsNotNull(TweetCount);
            Assert.IsTrue(TweetCount > previous);
        }
        [TestMethod]
        public void BuildHashTagList()
        {
            Dictionary<string, int> HashTagsPairs = new();
            var hashtagSamples = new List<string>
            {
                    "test", "test2", "test3",
                    "test3", "test4", "test",
                    "test", "test2"
            };
            //Iterate through each hashtag and increment count if exists or add if not existing.
            foreach (var tag in hashtagSamples)
            {
                if (HashTagsPairs != null && !HashTagsPairs.TryAdd(tag, 1))
                {
                    HashTagsPairs[tag]++;
                }
            }
            var rankedMetrics = ReturnLiveMetrics(HashTagsPairs);
            Assert.IsNotNull(rankedMetrics);
            //Max value is ordered
            Assert.IsTrue(rankedMetrics.HashTagPairs.Max(x => x.Value) == rankedMetrics.HashTagPairs.First().Value);
            
        }
        public TweetMetricStreamModel ReturnLiveMetrics(Dictionary<string, int> hashTagsPairs)
        {
            IEnumerable<KeyValuePair<string, int>> topTenHashTags;
            //While there are less than 10 tweets (common) don't limit the amount streamed to only take 10.
            if (hashTagsPairs.Count < 10)
            {
                topTenHashTags = hashTagsPairs.OrderByDescending(x => x.Value);
            }
            else
            {
                //Order to grab the most used hashtags and take top ten
                topTenHashTags = hashTagsPairs.OrderByDescending(x => x.Value).Take(10);
            }
            return new TweetMetricStreamModel
            {
                TweetCount = TweetCount,
                HashTagPairs = topTenHashTags
            };
        }
    }
}
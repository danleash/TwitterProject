using System.Text.Json;
using System.Text.Json.Serialization;
namespace TwitterProject.Shared.Models
{
    public class TwitterResponseModels
    {
    }
    public class TweetModel
    {
        public string TweetHeader { get; set; }
        public string Id { get; set; }
        public string Text { get; set; }
        public string Language { get; set; }
        public Domain Domain { get; set; }
        public EntityModel Entity { get; set; }
        public EntitiesModel Entities { get; set; }
        public DateTime DateTweeted { get; set; } = DateTime.UtcNow;
    }

    public class TweetMetricStreamModel
    {
        public IEnumerable<KeyValuePair<string, int>> HashTagPairs { get; set; }
        public int TweetCount { get; set; }
    }

    public class TwitterStreamResponse
    {
        [JsonPropertyName("data")]
        public StreamDataModel Data { get; set; }
    }

    public class StreamDataModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("context_annotations")]
        public ContextAnnotation[] ContextAnnotations { get; set; }
        [JsonPropertyName("entities")]
        public EntitiesModel Entities { get; set; }
        [JsonPropertyName("lang")]
        public string LanguageCode { get; set; }
    }

    public class ContextAnnotation
    {
        [JsonPropertyName("domain")]
        public Domain Domain { get; set; }
        [JsonPropertyName("entity")]
        public EntityModel Entity { get; set; }
    }
    public class Domain
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("hashtags")]
        public HashTagModel[] Hashtags { get; set; }
    }

    public class EntitiesModel
    {
        [JsonPropertyName("annotations")]
        public AnnotationModel[] Annotations { get; set; }
        [JsonPropertyName("urls")]
        public URLModel[] Urls { get; set; }
        [JsonPropertyName("hashtags")]
        public HashTagModel[] Hashtags { get; set; }
        [JsonPropertyName("mentions")]
        public MentionModel[] Mentions { get; set; }
        [JsonPropertyName("cashtags")]
        public CashTagModel[] CashTags { get; set; }
    }

    public class AnnotationModel
    {
        [JsonPropertyName("start")]
        public int Start { get; set; }
        [JsonPropertyName("end")]
        public int End { get; set; }
        [JsonPropertyName("probability")]
        public double Probability { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("normalized_text")]
        public string NormalizedText { get; set; }
    }

    public class URLModel
    {
        [JsonPropertyName("start")]
        public int Start { get; set; }
        [JsonPropertyName("end")]
        public int End { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("expanded_url")]
        public string UrlExpanded { get; set; }
        [JsonPropertyName("display_url")]
        public string DisplayUrl { get; set; }
        [JsonPropertyName("media_key")]
        public string MediaKey { get; set; }
    }

    public class MentionModel
    {
        [JsonPropertyName("start")]
        public int Start { get; set; }
        [JsonPropertyName("end")]
        public int End { get; set; }
        [JsonPropertyName("username")]
        public string UserName { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class EntityModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("hashtags")]
        public HashTagModel[] HashTags { get; set; }
    }

    public class HashTagModel
    {
        [JsonPropertyName("tag")]
        public string Tag { get; set; }
        [JsonPropertyName("start")]
        public int Start { get; set; }
        [JsonPropertyName("end")]
        public int End { get; set; }
    }

    public class CashTagModel
    {
        [JsonPropertyName("tag")]
        public string Tag { get; set; }
        [JsonPropertyName("start")]
        public int Start { get; set; }
        [JsonPropertyName("end")]
        public int End { get; set; }
    }
}

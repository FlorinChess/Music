using Newtonsoft.Json;

namespace Music.APIs.Spotify.Models
{
    public sealed record Item
    {
        [JsonProperty(PropertyName = "album")]
        public Album Album { get; set; }

        [JsonProperty(PropertyName = "artists")]
        public Artist[] Artists { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

}

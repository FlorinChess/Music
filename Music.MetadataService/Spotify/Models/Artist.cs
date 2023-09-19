using Newtonsoft.Json;

namespace Music.APIs.Spotify.Models
{
    public sealed record Artist
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}

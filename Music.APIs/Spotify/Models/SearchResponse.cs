using Newtonsoft.Json;

namespace Music.APIs.Spotify.Models
{
    public sealed record SearchResponse
    {
        [JsonProperty(PropertyName = "tracks")]
        public Tracks Tracks { get; set; }
    }
}

using Newtonsoft.Json;

namespace Music.APIs.Spotify.Models
{
    public sealed record Tracks
    {
        [JsonProperty(PropertyName = "items")]
        public Item[] Items { get; set; }
    }
}

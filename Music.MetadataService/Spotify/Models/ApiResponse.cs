using Newtonsoft.Json;

namespace Music.APIs.Spotify.Models
{
    public sealed record ApiResponse
    {
        [JsonProperty(PropertyName = "tracks")]
        public Tracks Tracks { get; set; }
    }
}

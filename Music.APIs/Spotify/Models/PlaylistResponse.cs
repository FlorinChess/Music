using Newtonsoft.Json;

namespace Music.APIs.Spotify.Models
{
    public sealed record PlaylistResponse
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tracks")]
        public PlaylistTracks PlaylistTracks { get; set; }
    }
}

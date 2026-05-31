using Newtonsoft.Json;

namespace Music.APIs.Spotify.Models
{
    public sealed record PlaylistTrackObject
    {
        [JsonProperty(PropertyName = "track")]
        public Track Track { get; set; }
    }
}

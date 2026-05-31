using Newtonsoft.Json;

namespace Music.APIs.Spotify.Models
{
    public sealed record PlaylistTracks
    {
        [JsonProperty(PropertyName = "items")]
        public PlaylistTrackObject[] PlaylistTrackObjects { get; set; }
    }
}

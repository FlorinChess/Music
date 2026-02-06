using Newtonsoft.Json;

namespace Music.APIs.Spotify.Models
{
    public sealed record Album
    {
        [JsonProperty(PropertyName = "album_type")]
        public string AlbumType { get; set; }

        [JsonProperty(PropertyName = "total_tracks")]
        public int TotalTracks { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "images")]
        public Image[] Images { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty(PropertyName = "artists")]
        public Artist[] Artists { get; set; }
    }
}

using Newtonsoft.Json;

namespace Music.APIs.Spotify.Models
{
    public sealed record Image
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "height")]
        public int Height { get; set; }

        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }
    }

}

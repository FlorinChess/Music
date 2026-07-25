using System.Text.Json.Serialization;

namespace Music.APIs.Spotify.Models;

public sealed record Artist
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

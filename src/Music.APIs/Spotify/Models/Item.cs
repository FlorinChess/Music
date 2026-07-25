using System.Text.Json.Serialization;

namespace Music.APIs.Spotify.Models;

public sealed record Item
{
    [JsonPropertyName("album")]
    public Album Album { get; set; }

    [JsonPropertyName("artists")]
    public Artist[] Artists { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

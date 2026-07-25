using System.Text.Json.Serialization;

namespace Music.APIs.Spotify.Models;

public sealed record Tracks
{
    [JsonPropertyName("items")]
    public Item[] Items { get; set; }
}

using System.Text.Json.Serialization;

namespace Music.APIs.Spotify.Models;

public sealed record ApiResponse
{
    [JsonPropertyName("tracks")]
    public Tracks Tracks { get; set; }
}

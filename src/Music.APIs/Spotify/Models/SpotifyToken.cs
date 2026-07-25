using System.Text.Json.Serialization;

namespace Music.APIs.Spotify.Models;

public sealed record SpotifyToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}

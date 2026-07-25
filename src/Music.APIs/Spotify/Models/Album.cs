using System.Text.Json.Serialization;

namespace Music.APIs.Spotify.Models;

public sealed record Album
{
    [JsonPropertyName("album_type")]
    public string AlbumType { get; set; }

    [JsonPropertyName("total_tracks")]
    public int TotalTracks { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("images")]
    public Image[] Images { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("release_date")]
    public string ReleaseDate { get; set; }

    [JsonPropertyName("artists")]
    public Artist[] Artists { get; set; }
}

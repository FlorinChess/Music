using System.Xml.Serialization;

namespace Music.Domain.DataModels
{
    [Serializable]
    [XmlRoot("Root")]
    public sealed record Root
    {
        [XmlArray("Playlists")]
        [XmlArrayItem("Playlist")]
        public List<Playlist> Playlists { get; set; } = new();
    }
}

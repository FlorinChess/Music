using System.Xml.Serialization;

namespace Music.Domain.DataModels
{
    [Serializable]
    [XmlType("Playlist")]
    public sealed record Playlist
    {
        public string Name { get; set; }
        public string DateCreatedString { get; set; }
        public string ImagePath { get; set; }

        [XmlArray("Tracks")]
        [XmlArrayItem("FilePath")]
        public List<string> TracksFilePaths { get; set; }
    }
}

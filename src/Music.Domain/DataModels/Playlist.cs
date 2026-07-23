using System.Xml.Serialization;

namespace Music.Domain.DataModels;

[Serializable]
[XmlType("Playlist")]
public sealed record Playlist
{
    public string Name { get; set; }
    public DateOnly DateCreated { get; set; }
    public string ImagePath { get; set; }

    [XmlArray("Tracks")]
    [XmlArrayItem("FilePath")]
    public List<string> TracksFilePaths { get; set; }
}

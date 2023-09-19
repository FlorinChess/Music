using System;
using System.Xml.Serialization;

namespace Music.WPF.Models
{
    [Serializable]
    [XmlType("Playlist")]
    public sealed record PersistenceObject
    {
        public string Name { get; set; }
        public string DateCreatedString { get; set; }

        [XmlArray("Tracks")]
        [XmlArrayItem("FilePath")]
        public string[] TracksFilePaths { get; set; }
        public string ImagePath { get; set; }
    }
}

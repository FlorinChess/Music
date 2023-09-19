using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Music.WPF.Models
{
    [Serializable]
    [XmlRoot("Root")]
    public sealed record PersistenceObjectList
    {
        [XmlArray("Playlists")]
        [XmlArrayItem("Playlist")]
        public List<PersistenceObject> Playlists { get; set; }
    }
}

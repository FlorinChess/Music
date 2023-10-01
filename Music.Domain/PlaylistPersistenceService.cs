using Music.Domain.DataModels;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Music.Domain
{
    public sealed class PlaylistPersistenceService : IPersistenceService
    {
        #region Private Members

        private readonly string _saveFolderPath;
        private readonly Root _rootObject;
        private readonly XmlSerializer _serializer;

        #endregion Private Members

        public string SaveFilePath { get; set; }

        public PlaylistPersistenceService()
        {
            _rootObject = new();
            _serializer = new(typeof(Root));

            _saveFolderPath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "\\Music\\Playlists\\");
            
            SaveFilePath = string.Concat(_saveFolderPath, "playlists.xml");
        }

        #region Public Methods

        public Root? Parse()
        {
            if (!File.Exists(SaveFilePath)) return null;

            try
            {
                using var fileStream = new StreamReader(SaveFilePath);

                return (Root?)_serializer.Deserialize(fileStream);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Save()
        {
            try
            {
                using var fileStream = new FileStream(SaveFilePath, FileMode.Create);

                var writer = new XmlTextWriter(fileStream, Encoding.Unicode);

                _serializer.Serialize(writer, _rootObject);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Add(string name, string dateCreated, string imagePath, List<string> trackFilePaths)
        {
            _rootObject.Playlists.Add(new Playlist()
            {
                Name = name,
                DateCreatedString = dateCreated,
                ImagePath = imagePath,
                TracksFilePaths = trackFilePaths
            });
        }

        #endregion Public Methods
    }
}

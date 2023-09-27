using Music.WPF.Models;
using Music.WPF.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Music.WPF.Services
{
    public sealed class PersistenceService : IPersistenceService
    {
        #region Private Members

        private readonly string _saveFolderPath;
        private readonly string _saveFilePath;
        private readonly TrackStore _trackStore;
        private readonly XmlSerializer _serializer;

        #endregion Private Members

        public PersistenceService(TrackStore trackStore)
        {
            _trackStore = trackStore;
            _serializer = new(typeof(PersistenceObjectList));

            _saveFolderPath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "\\Music\\Playlists\\");
            _saveFilePath = string.Concat(_saveFolderPath, "playlists.xml");
        }

        #region Private Methods

        private static List<PersistenceObject> CreatePersistenceObjects(List<PlaylistModel> playlists)
        {
            var persistenceObjects = new List<PersistenceObject>();

            for (int i = 0; i < playlists.Count; i++)
            {
                var playlist = playlists[i];

                persistenceObjects.Add(new PersistenceObject()
                {
                    Name = playlist.Name,
                    DateCreatedString = playlist.DateCreated.ToString(),
                    TracksFilePaths = playlist.Tracks.Select(x => x.FilePath).ToArray(),
                    ImagePath = playlist.ImagePath
                });
            }

            return persistenceObjects;
        }

        private void AddPlaylistsToStore(IList<PlaylistModel> playlists)
        {
            for (int i = 0; i < playlists.Count;i++)
            {
                _trackStore.AddPlaylist(playlists[i]);
            }
        }

        private void AddTracksToPlaylist(IList<string> filePaths, PlaylistModel playlist)
        {
            try
            {
                for( int i = 0; i < filePaths.Count; i++)
                {
                    var track = _trackStore.GetTrackByFilePath(filePaths[i]);
                    playlist.Tracks.Add(track);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        #endregion

        public void Parse()
        {
            if (!File.Exists(_saveFilePath)) return;

            using var fileStream = new StreamReader(_saveFilePath);

            var persistenceObjectList = (PersistenceObjectList?)_serializer.Deserialize(fileStream);

            var playlists = new List<PlaylistModel>();

            for (int i = 0; i < persistenceObjectList?.Playlists.Count; i++)
            {
                var playlistObject = persistenceObjectList.Playlists[i];

                var playlist = new PlaylistModel()
                {
                    Name = playlistObject.Name,
                    ImagePath = playlistObject.ImagePath,
                };

                playlists.Add(playlist);

                AddTracksToPlaylist(playlistObject.TracksFilePaths, playlist);
            }

            AddPlaylistsToStore(playlists);
        }

        public void Save()
        {
            if (!_trackStore.PlaylistsChanged) return;

            try
            {
                var persistenceObjectList = new PersistenceObjectList() { Playlists = CreatePersistenceObjects(_trackStore.AvailablePlaylists) };

                var fileStream = new FileStream(_saveFilePath, FileMode.Create);

                var writer = new XmlTextWriter(fileStream, Encoding.Unicode);

                _serializer.Serialize(writer, persistenceObjectList);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

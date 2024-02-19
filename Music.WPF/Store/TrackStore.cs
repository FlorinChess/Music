using Music.WPF.Models;
using Music.WPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Music.WPF.Store
{
    public sealed class TrackStore
    {
        #region Public Events

        public event Action AvailablePlaylistsChanged;
        public event Action<IList<TrackModel>> AvailableTracksChanged;
        public event Action CurrentTrackChanged;
        public event Action QueueChanged;

        #endregion Public Events

        #region Properties

        private TrackModel? _currentTrack;
        public TrackModel? CurrentTrack
        {
            get => _currentTrack;
            set
            {
                if (_currentTrack is not null)
                    _currentTrack.IsCurrentlyPlaying = false;

                _currentTrack = value;

                if (_currentTrack is not null)
                    _currentTrack.IsCurrentlyPlaying = true;

                CurrentTrackChanged?.Invoke();
            }
        }

        private PlaylistModel _currentPlaylist;
        public PlaylistModel CurrentPlaylist
        {
            get => _currentPlaylist;
            set
            {
                _currentPlaylist = value;
            }
        }

        public List<TrackModel> Queue { get; private set; } = new();
        public List<TrackModel> AvailableTracks { get; private set; } = new();
        public List<PlaylistModel> AvailablePlaylists { get; private set; } = new();
        public List<MusicFolderModel> MusicFolders { get; private set; } = new();
        public bool PlaylistsChanged { get; private set; } = false;

        #endregion

        #region Private Methods

        private void QueueUpdated()
        {
            QueueChanged?.Invoke();

            if (_currentTrack is null && Queue.Count != 0)
            {
                CurrentTrack = Queue[0];
            }
        }

        private void MusicFoldersUpdated(MusicFolderModel musicFolder)
        {

            var musicFilePaths = MusicFilesService.GetMusicFiles(musicFolder.Path);

            var tracks = TrackFactory.CreateTracks(musicFilePaths);

            AddTracks(tracks);

            AvailableTracksChanged?.Invoke(tracks);
        }

        private void AddTracksToQueue(IList<TrackModel> tracks)
        {
            for (int i = 0; i < tracks.Count; i++)
            {
                Queue.Add(tracks[i]);
            }
        }

        private void AddPlaylistsToAvailablePlaylists(IList<PlaylistModel> playlists)
        {
            for(int i = 0; i <  playlists.Count; i++)
            {
                AvailablePlaylists.Add(playlists[i]);
            }
        }

        #endregion

        #region Public Methods

        public void SetQueue(IList<TrackModel> tracks, bool setFirstAsCurrent = true)
        {
            if (tracks is null || tracks.Count == 0) return;

            Queue.Clear();

            AddTracksToQueue(tracks);

            if (Queue.Count > 0 && setFirstAsCurrent)
                CurrentTrack = Queue[0];

            QueueUpdated();
        }

        public void SetQueue(TrackModel track, bool setFirstAsCurrent = true)
        {
            Queue.Clear();

            Queue.Add(track);

            if (Queue.Count > 0 && setFirstAsCurrent)
                CurrentTrack = Queue[0];

            QueueUpdated();
        }

        public void AddToQueue(IList<TrackModel> tracks)
        {
            if (tracks is null || tracks.Count == 0) return;

            AddTracksToQueue(tracks);

            QueueUpdated();
        }

        public void AddToQueue(TrackModel track)
        {
            Queue.Add(track);

            QueueUpdated();
        }

        public void AddNextInQueue(TrackModel track)
        {
            var currentTrackIndex = Queue.IndexOf(CurrentTrack!);

            Queue.Insert(currentTrackIndex + 1, track);

            QueueUpdated();
        }

        public void ClearQueue()
        {
            if (Queue is null || Queue.Count == 0) return;

            Queue.Clear();

            CurrentTrack = null;

            QueueUpdated();
        }

        public void SetCurrentPlaylist(PlaylistModel playlist)
        {
            CurrentPlaylist = playlist;
        }

        public void AddPlaylist(PlaylistModel playlist)
        {
            AvailablePlaylists.Add(playlist);
            PlaylistsUpdated();
        }

        public void AddPlaylists(IList<PlaylistModel> playlists)
        {
            if (playlists is null || playlists.Count == 0) return;

            AddPlaylistsToAvailablePlaylists(playlists);

            PlaylistsUpdated();
        }

        public void RemovePlaylist(PlaylistModel playlist)
        {
            if (AvailablePlaylists.Count == 0) return;

            AvailablePlaylists.Remove(playlist);
            PlaylistsUpdated();
        }

        public void AddTracks(IList<TrackModel> tracks)
        {
            if (tracks is null || tracks.Count == 0) return;

            for (int i = 0; i < tracks.Count; i++)
            {
                AvailableTracks.Add(tracks[i]);
            }
        }

        public void SetTrackAsCurrent(TrackModel track)
        {
            if (CurrentTrack == track) return;

            CurrentTrack = track;
        }

        public void PlaylistsUpdated()
        {
            AvailablePlaylistsChanged?.Invoke();

            PlaylistsChanged = true;
        }

        public TrackModel GetTrackByFilePath(string filePath)
        {
            return AvailableTracks.Where(t => t.FilePath == filePath).Single();
        }

        public void AddMusicFolder(MusicFolderModel musicFolder)
        {
            MusicFolders.Add(musicFolder);

            MusicFoldersUpdated(musicFolder);
        }

        #endregion
    }
}

using Music.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Music.WPF.Store
{
    public sealed class TrackStore
    {
        #region Public Events

        public event Action AvailablePlaylistsChanged;
        public event Action CurrentTrackChanged;
        public event Action MusicFoldersChanged;
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

        private void QueueChange()
        {
            QueueChanged?.Invoke();

            if (_currentTrack is null && Queue.Count != 0)
            {
                CurrentTrack = Queue[0];
            }
        }

        private void AddTracksToQueue(IList<TrackModel> tracks)
        {
            for (int i = 0; i < tracks.Count; i++)
            {
                Queue.Add(tracks[i]);
            }
        }

        #endregion

        #region Public Methods

        public void SetQueue(IList<TrackModel> tracks, bool setFirstAsCurrent = true)
        {
            if (tracks is null || tracks.Count == 0) return;

            Queue.Clear();

            AddTracksToQueue(tracks);

            if (setFirstAsCurrent)
                CurrentTrack = Queue[0];

            QueueChange();
        }

        public void SetQueue(TrackModel track, bool setFirstAsCurrent = true)
        {
            Queue.Clear();

            Queue.Add(track);

            if (setFirstAsCurrent)
                CurrentTrack = Queue[0];

            QueueChange();
        }

        public void AddToQueue(IList<TrackModel> tracks)
        {
            if (tracks is null || tracks.Count == 0) return;

            AddTracksToQueue(tracks);

            QueueChange();
        }

        public void AddToQueue(TrackModel track)
        {
            Queue.Add(track);

            QueueChange();
        }

        public void AddNextInQueue(TrackModel track)
        {
            var currentTrackIndex = Queue.IndexOf(CurrentTrack!);

            Queue.Insert(currentTrackIndex + 1, track);

            QueueChange();
        }

        public void ClearQueue()
        {
            if (Queue is null || Queue.Count == 0) return;

            Queue.Clear();

            CurrentTrack = null;

            QueueChange();
        }

        public void SetCurrentPlaylist(PlaylistModel playlist)
        {
            CurrentPlaylist = playlist;
        }

        public void AddPlaylist(PlaylistModel playlist)
        {
            AvailablePlaylists.Add(playlist);
            PlaylistUpdated();
        }

        public void AddTracks(IList<TrackModel> tracks)
        {
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

        public void RemovePlaylist(PlaylistModel playlist)
        {
            AvailablePlaylists.Remove(playlist);
            PlaylistUpdated();
        }

        public void PlaylistUpdated()
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

            MusicFoldersChanged?.Invoke();
        }

        #endregion
    }
}

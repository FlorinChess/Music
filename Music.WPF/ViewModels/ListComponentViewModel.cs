using Music.WPF.Commands;
using Music.WPF.Modals.ViewModels;
using Music.WPF.Models;
using Music.WPF.Store;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Music.WPF.ViewModels
{
    public sealed class ListComponentViewModel : BaseViewModel
    {
        public event Action TracksChanged;

        #region Private Members

        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly TrackStore _trackStore;

        #endregion

        #region Properties

        public int Count => Tracks.Count;
        public ObservableCollection<TrackModel> Tracks { get; private set; } = new();

        private TrackModel _selectedTrack;
        public TrackModel SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                _selectedTrack = value;
                OnPropertyChanged(nameof(SelectedTrack));
            }
        }

        private PlaylistModel? _playlist;
        public PlaylistModel? Playlist
        {
            get => _playlist;
            private set
            {
                _playlist = value;
                OnPropertyChanged(nameof(Playlist));
            }
        }

        private bool _canRemoveTrack = true;
        public bool CanRemoveTrack
        {
            get => _canRemoveTrack;
            set
            {
                _canRemoveTrack = value;
                OnPropertyChanged(nameof(CanRemoveTrack));
            }
        }

        private bool _contextMenuEnabled;
        public bool ContextMenuEnabled
        {
            get => _contextMenuEnabled;
            set
            {
                _contextMenuEnabled = value;
                OnPropertyChanged(nameof(ContextMenuEnabled));
            }
        }

        #endregion

        #region Commands

        public ICommand ShufflePlayAllCommand { get; private set; }
        public ICommand PlayAllCommand { get; private set; }
        public ICommand PlayCommand { get; set; }
        public ICommand PlayNextCommand { get; }
        public ICommand AddToQueueCommand { get; }
        public ICommand AddToPlaylistCommand { get; }
        public ICommand RemoveTrackCommand { get; set; }

        #endregion

        public ListComponentViewModel(ModalNavigationStore modalNavigationStore, TrackStore trackStore)
        {
            _modalNavigationStore = modalNavigationStore;
            _trackStore = trackStore;

            PlayCommand = new RelayCommand(_ => PlaySelectedTrack());
            PlayNextCommand = new RelayCommand(_ => PlayNext());
            PlayAllCommand = new RelayCommand(PlayAllTracks);
            AddToQueueCommand = new RelayCommand(_ => AddToQueue());
            RemoveTrackCommand = new RelayCommand(_ => RemoveTrack(SelectedTrack));
            AddToPlaylistCommand = new RelayCommand(_ => AddToPlaylist());
            ShufflePlayAllCommand = new RelayCommand(_ => ShufflePlayAll());

            Playlist = _trackStore.CurrentPlaylist;
        }

        #region Private Methods

        private void AddTracksToList(IList<TrackModel> tracks)
        {
            for (int i = 0; i < tracks.Count; i++)
            {
                Tracks.Add(tracks[i]);
            }

            ContextMenuEnabled = Count > 0;
        }

        private void AddToPlaylist()
        {
            if (SelectedTrack is null) return;

            _modalNavigationStore.CurrentViewModel = new AddToPlaylistModalViewModel(_modalNavigationStore, _trackStore, SelectedTrack);
        }

        private void PlayNext()
        {
            _trackStore.AddNextInQueue(_selectedTrack);
        }

        private void AddToQueue()
        {
            _trackStore.AddToQueue(_selectedTrack);
        }

        private void PlayAllTracks(object? parameter)
        {
            if (parameter is null)
            {
                _trackStore.SetQueue(Tracks);
            }
            else
            {
                _trackStore.SetQueue(PlayAllSelectedFirst(Tracks, SelectedTrack));
            }
        }

        private static List<TrackModel> PlayAllSelectedFirst(ObservableCollection<TrackModel> tracks, TrackModel selectedTrack)
        {
            var tracksToBePlayed = new List<TrackModel>();
            var selectedTrackIndex = tracks.IndexOf(selectedTrack);

            for (int i = selectedTrackIndex; i < tracks.Count; i++)
            {
                tracksToBePlayed.Add(tracks[i]); // Bug i = -1
            }

            for (int i = 0; i < selectedTrackIndex; i++)
            {
                tracksToBePlayed.Add(tracks[i]);
            }

            return tracksToBePlayed;
        }

        private void PlaySelectedTrack()
        {
            if (_selectedTrack is null) return;

            _trackStore.SetQueue(_selectedTrack);
        }

        private void ShufflePlayAll()
        {
            var allTracks = new List<TrackModel>(Tracks);
            var shuffledTracks = new List<TrackModel>();
            var random = new Random();
            var count = allTracks.Count;

            for (int i = 0; i < count; i++)
            {
                int randomIndex = random.Next(allTracks.Count);

                shuffledTracks.Add(allTracks[randomIndex]);
                allTracks.Remove(allTracks[randomIndex]);
            }

            _trackStore.SetQueue(shuffledTracks);
        }

        #endregion

        #region Public Methods

        public void SetTracks(IList<TrackModel> tracks)
        {
            if (tracks is null || tracks.Count == 0) return;

            Tracks.Clear();

            AddTracksToList(tracks);
        }

        public void SetPlaylist(PlaylistModel playlist)
        {
            Playlist = playlist;

            AddTracksToList(playlist.Tracks);
        }

        public void ClearTracks()
        {
            Tracks.Clear();
            ContextMenuEnabled = Count > 0;
        }

        public void AddTrack(TrackModel track)
        {
            Tracks.Add(track);
            ContextMenuEnabled = Count > 0;
        }

        public void AddTracks(IList<TrackModel> tracks)
        {
            if (tracks is null || tracks.Count == 0) return;

            AddTracksToList(tracks);
        }

        public void RemoveTrack(TrackModel track)
        {
            Tracks.Remove(track);
            ContextMenuEnabled = Count > 0;
            TracksChanged?.Invoke();
        }

        #endregion Public Methods
    }
}

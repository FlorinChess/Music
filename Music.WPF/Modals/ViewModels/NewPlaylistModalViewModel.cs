using Music.WPF.Commands;
using Music.WPF.Models;
using Music.WPF.Services;
using Music.WPF.Store;
using Music.WPF.ViewModels;
using System;
using System.Windows.Input;

namespace Music.WPF.Modals.ViewModels
{
    public sealed class NewPlaylistModalViewModel : BaseViewModel, IModal, IRequestFocus
    {
        public event EventHandler<FocusRequestedEventArgs> FocusRequested;

        #region Private Members

        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly TrackStore _trackStore;

        #endregion Private Members

        #region Properties

        private string _playlistName = string.Empty;
        public string PlaylistName
        {
            get => _playlistName;
            set 
            { 
                _playlistName = value; 
                OnPropertyChanged(nameof(PlaylistName));
            }
        }

        #endregion Properties

        #region Commands

        public CloseModalCommand CloseModalCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        #endregion Commands

        public NewPlaylistModalViewModel(ModalNavigationStore modalNavigationStore, TrackStore trackStore)
        {
            _modalNavigationStore = modalNavigationStore;
            _trackStore = trackStore;

            CloseModalCommand = modalNavigationStore.CloseModalCommand;
            SaveCommand = new RelayCommand(_ => AddNewPlaylist(PlaylistName));
        }

        private void AddNewPlaylist(string playlistName)
        {
            if (string.IsNullOrEmpty(playlistName))
            {
                FocusRequested?.Invoke(this, new FocusRequestedEventArgs(nameof(PlaylistName)));
                return;
            }
            
            try
            {
                var newPlaylist = new PlaylistModel() { Name = playlistName };

                _trackStore.AddPlaylist(newPlaylist);

                _modalNavigationStore.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

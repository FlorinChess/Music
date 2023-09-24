using Microsoft.Extensions.DependencyInjection;
using Music.WPF.Commands;
using Music.WPF.Extensions;
using Music.WPF.Modals.ViewModels;
using Music.WPF.Models;
using Music.WPF.Store;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Music.WPF.ViewModels
{
    public sealed class PlaylistCollectionViewModel : BaseViewModel
    {
        #region Private Members

        private readonly IServiceProvider _serviceProvider;
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly NavigationStore _navigationStore;
        private readonly TrackStore _trackStore;

        #endregion

        #region Properties

        public ObservableCollection<PlaylistModel> Playlists { get; private set; } = new();

        public PlaylistModel SelectedPlaylist
        {
            get => null;
            set 
            { 
                _trackStore.SetCurrentPlaylist(value);
                NavigateToSelectedPlaylist();
            }
        }

        private string _numberOfPlaylists = "0 Playlists";
        public string NumberOfPlaylists
        {
            get => _numberOfPlaylists;
            set
            {
                _numberOfPlaylists = value;
                OnPropertyChanged(nameof(NumberOfPlaylists));
            }
        }

        #endregion Properties

        #region Commands

        public ICommand NewPlaylistCommand { get; set; }

        #endregion Commands

        public PlaylistCollectionViewModel(IServiceProvider serviceProvider, ModalNavigationStore modalNavigationStore, NavigationStore navigationStore, TrackStore trackStore)
        {
            _serviceProvider = serviceProvider;
            _modalNavigationStore = modalNavigationStore;
            _navigationStore = navigationStore;
            _trackStore = trackStore;
            _trackStore.AvailablePlaylistsChanged += OnAvailablePlaylistsChanged;

            Playlists.AddRange(_trackStore.AvailablePlaylists);

            UpdateNumberOfPlaylists(Playlists.Count);

            NewPlaylistCommand = new RelayCommand(_ => NavigateToNewPlaylist());
        }

        #region Private Methods

        private void OnAvailablePlaylistsChanged()
        {
            Playlists.Clear();

            Playlists.AddRange(_trackStore.AvailablePlaylists);

            UpdateNumberOfPlaylists(Playlists.Count);
        }

        private void UpdateNumberOfPlaylists(int numberOfPlaylists)
        {
            NumberOfPlaylists = (numberOfPlaylists == 1) ? $"{numberOfPlaylists} Playlist" : $"{numberOfPlaylists} Playlists";
        }

        private void NavigateToSelectedPlaylist()
        {
            _navigationStore.CurrentViewModel = _serviceProvider.GetRequiredService<SelectedPlaylistViewModel>();
        }

        private void NavigateToNewPlaylist()
        {
            _modalNavigationStore.CurrentViewModel = _serviceProvider.GetRequiredService<NewPlaylistModalViewModel>();
        }

        #endregion

        public override void Dispose()
        {
            _trackStore.AvailablePlaylistsChanged -= OnAvailablePlaylistsChanged;

            base.Dispose();
        }
    }
}

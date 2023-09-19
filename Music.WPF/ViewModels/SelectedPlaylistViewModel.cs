using Microsoft.Extensions.DependencyInjection;
using Music.WPF.Commands;
using Music.WPF.Modals.ViewModels;
using Music.WPF.Models;
using Music.WPF.Store;
using System;
using System.Windows.Input;

namespace Music.WPF.ViewModels
{
    public sealed class SelectedPlaylistViewModel : BaseViewModel
    {
        #region Private Members

        private readonly IServiceProvider _serviceProvider;
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly NavigationStore _navigationStore;
        private readonly TrackStore _trackStore;

        #endregion

        #region Properties

        public bool IsDropDownOpen { get; set; }
        public ListComponentViewModel ListComponentViewModel { get; set; }

        private PlaylistModel _selectedPlaylist;
        public PlaylistModel SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                _selectedPlaylist = value;
                OnPropertyChanged(nameof(SelectedPlaylist));
            }
        }

        private string _placeholderText = string.Empty;
        public string PlaceholderText
        {
            get => _placeholderText;
            set 
            { 
                _placeholderText = value;
                OnPropertyChanged(nameof(PlaceholderText));
            }
        }


        private string _numberOfTracks;
        public string NumberOfTracks 
        { 
            get => _numberOfTracks; 
            private set
            {
                _numberOfTracks = value; 
                OnPropertyChanged(nameof(NumberOfTracks));
            }
        }

        private string _playTime;

        public string PlayTime
        {
            get => _playTime;
            set
            {
                _playTime = value;
                OnPropertyChanged(nameof(PlayTime));
            }
        }

        #endregion

        #region Commands

        public ICommand PlayAllCommand { get; }
        public ICommand EditPlaylistCommand { get; }
        public ICommand AddPlaylistToFavoritesCommand { get; }
        public ICommand DeletePlaylistCommand { get; }

        #endregion Commands

        public SelectedPlaylistViewModel(IServiceProvider serviceProvider, ModalNavigationStore modalNavigationStore, NavigationStore navigationStore, TrackStore trackStore, ListComponentViewModel listComponentViewModel)
        {
            _serviceProvider = serviceProvider;
            _modalNavigationStore = modalNavigationStore;
            _navigationStore = navigationStore;
            _trackStore = trackStore;
            _trackStore.AvailablePlaylistsChanged += OnAvailablePlaylistsChanged;

            SelectedPlaylist = _trackStore.CurrentPlaylist;

            ListComponentViewModel = listComponentViewModel;
            ListComponentViewModel.SetPlaylist(SelectedPlaylist);
            ListComponentViewModel.SetTracks(SelectedPlaylist.Tracks);
            ListComponentViewModel.TracksChanged += OnTracksChanged;

            PlayAllCommand = ListComponentViewModel.PlayAllCommand;
            EditPlaylistCommand = new RelayCommand(_ => EditPlaylist());
            DeletePlaylistCommand = new RelayCommand(_ => DeletePlaylist());

            // Custom remove behavior for the ListComponent.RemoveCommand
            ListComponentViewModel.RemoveTrackCommand = new RelayCommand(_ => RemoveTrack());

            UpdateTracklistInformation();
        }

        #region Private Methods

        private void DeletePlaylist()
        {
            IsDropDownOpen = false;
            OnPropertyChanged(nameof(IsDropDownOpen));

            _modalNavigationStore.CurrentViewModel = 
                new ConfirmationModalViewModel(_modalNavigationStore, $"remove \"{SelectedPlaylist.Name}\"", 
                () =>
                {
                    _trackStore.RemovePlaylist(SelectedPlaylist);

                    _navigationStore.CurrentViewModel = _serviceProvider.GetRequiredService<PlaylistCollectionViewModel>();
                });
        }

        private void OnAvailablePlaylistsChanged()
        {
            SelectedPlaylist = _trackStore.CurrentPlaylist;
        }

        private void OnTracksChanged()
        {
            UpdateTracklistInformation();
        }

        private void RemoveTrack()
        {
            SelectedPlaylist.Tracks.Remove(ListComponentViewModel.SelectedTrack);

            ListComponentViewModel.RemoveTrack(ListComponentViewModel.SelectedTrack);
        }

        private void UpdateTracklistInformation()
        {
            NumberOfTracks = (ListComponentViewModel.Count != 1) ? $"{ListComponentViewModel.Count} Tracks" : $"1 Track";
            PlayTime = GetStringTotalTimeFromTracks(ListComponentViewModel.Tracks);
            PlaceholderText = ListComponentViewModel.Count == 0 ? "There are no tracks in this playlist." : string.Empty;
        }

        private void EditPlaylist()
        {
            IsDropDownOpen = false;
            OnPropertyChanged(nameof(IsDropDownOpen));

            _modalNavigationStore.CurrentViewModel = new EditPlaylistModalViewModel(_modalNavigationStore, _trackStore, _selectedPlaylist);
        }

        #endregion

        public override void Dispose()
        {
            _trackStore.AvailablePlaylistsChanged -= OnAvailablePlaylistsChanged;
            ListComponentViewModel.TracksChanged -= OnTracksChanged;

            base.Dispose();
        }
    }
}

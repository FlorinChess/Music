using Microsoft.Extensions.DependencyInjection;
using Music.WPF.Commands;
using Music.WPF.Core;
using Music.WPF.Extensions;
using Music.WPF.Models;
using Music.WPF.Store;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Music.WPF.ViewModels
{
    public sealed class MyMusicViewModel : BaseViewModel, INavigation
    {
        #region Private Members

        private readonly IServiceProvider _serviceProvider;
        private readonly TrackStore _trackStore;
        private ICommand _addMusicFolderCommand;

        #endregion

        #region Properties

        public ListComponentViewModel ListComponentViewModel { get; set; }
        public static List<string> SortOptions => EnumExtension.GetSortOptions();

        private bool _placeholderVisibility;
        public bool PlaceholderVisibility
        {
            get => _placeholderVisibility;
            set
            {
                _placeholderVisibility = value;
                OnPropertyChanged(nameof(PlaceholderVisibility));
            }
        }

        private string _selectedSortOption;
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (value == _selectedSortOption) return;

                _selectedSortOption = value;
                OnPropertyChanged(nameof(SelectedSortOption));

                var orderedTracks = Sort(ListComponentViewModel.Tracks, _selectedSortOption);

                ListComponentViewModel.SetTracks(orderedTracks);
            }
        }


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

        private PlaylistModel _playlist;
        public PlaylistModel Playlist
        {
            get => _playlist;
            set
            {
                _playlist = value;
                OnPropertyChanged(nameof(Playlist));
            }
        }

        private string _numberOfTracks;
        public string NumberOfTracks
        {
            get => _numberOfTracks;
            set
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

        public ICommand ShufflePlayAllCommand { get; }
        public ICommand PlayAllCommand { get; }
        public ICommand AddMusicFolderCommand => _addMusicFolderCommand ??= new RelayCommand(_ => AddMusicFolder());

        #endregion

        public MyMusicViewModel(IServiceProvider serviceProvider, TrackStore trackStore, ListComponentViewModel listComponentViewModel)
        {
            _serviceProvider = serviceProvider;
            _trackStore = trackStore;
            _trackStore.AvailableTracksChanged += OnAvailableTrackChanged;

            ListComponentViewModel = listComponentViewModel;
            ListComponentViewModel.TracksChanged += OnTracksChanged;

            PlayAllCommand = ListComponentViewModel.PlayAllCommand;
            ShufflePlayAllCommand = ListComponentViewModel.ShufflePlayAllCommand;

            ListComponentViewModel.SetTracks(_trackStore.AvailableTracks);
            
            SelectedSortOption = SortOptions[0];

            UpdateTracklistInformation();
        }

        #region Private Methods

        private void OnAvailableTrackChanged(IList<TrackModel> newTracks)
        {
            ListComponentViewModel.AddTracks(newTracks);

            OnTracksChanged();
        }

        private void AddMusicFolder()
        {
            var viewModel = _serviceProvider.GetRequiredService<SettingsViewModel>();

            viewModel.SelectMusicFilesFolderCommand.Execute(null);
        }

        private void OnTracksChanged()
        {
            Task.Run(UpdateTracklistInformation);
        }

        private void UpdateTracklistInformation()
        {
            NumberOfTracks = (ListComponentViewModel.Count != 1) ? $"{ListComponentViewModel.Count} Tracks" : $"1 Track";
            PlayTime = GetStringTotalTimeFromTracks(ListComponentViewModel.Tracks);
            PlaceholderVisibility = ListComponentViewModel.Count == 0;
        }

        #endregion

        public override PageIndex GetPageIndex() => PageIndex.MyMusic;
    }
}

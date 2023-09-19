using Music.WPF.Extensions;
using Music.WPF.Models;
using Music.WPF.Store;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Music.WPF.ViewModels
{
    public sealed class MyMusicViewModel : BaseViewModel
    {
        #region Private Members

        private readonly TrackStore _trackStore;

        #endregion

        #region Properties

        public ListComponentViewModel ListComponentViewModel { get; set; }
        public static List<string> SortOptions => EnumExtension.GetSortOptions();

        private string _placeholderText;
        public string PlaceholderText
        {
            get => _placeholderText;
            set 
            { 
                _placeholderText = value; 
                OnPropertyChanged(nameof(PlaceholderText));
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

        #endregion

        public MyMusicViewModel(TrackStore trackStore, ListComponentViewModel listComponentViewModel)
        {
            _trackStore = trackStore;
            ListComponentViewModel = listComponentViewModel;
            ListComponentViewModel.TracksChanged += OnTracksChanged;

            PlayAllCommand = ListComponentViewModel.PlayAllCommand;
            ShufflePlayAllCommand = ListComponentViewModel.ShufflePlayAllCommand;

            ListComponentViewModel.SetTracks(_trackStore.AvailableTracks);
            
            ///For testing only!
            ///ListComponentViewModel.AddTrack(new TrackModel() { HasErrors = true, Title = "Test Track", Artist = "Test Artist", ErrorMessage = "This is a test error message" });

            SelectedSortOption = SortOptions[0];

            UpdateTracklistInformation();
        }

        #region Private Methods

        private void OnTracksChanged()
        {
            Task.Run(() => UpdateTracklistInformation());
        }

        private void UpdateTracklistInformation()
        {
            NumberOfTracks = (ListComponentViewModel.Count != 1) ? $"{ListComponentViewModel.Count} Tracks" : $"1 Track";
            PlayTime = GetStringTotalTimeFromTracks(ListComponentViewModel.Tracks);
            PlaceholderText = ListComponentViewModel.Count == 0 ? "No tracks here." : string.Empty;
        }

        #endregion

        public override void Dispose()
        {
            ListComponentViewModel.TracksChanged -= OnTracksChanged;

            base.Dispose();
        }
    }
}

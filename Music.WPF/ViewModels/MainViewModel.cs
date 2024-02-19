using Microsoft.Extensions.DependencyInjection;
using Music.Domain;
using Music.WPF.Store;
using System;
using System.Linq;
using System.Windows.Input;

namespace Music.WPF.ViewModels
{
    public sealed class MainViewModel : BaseViewModel
    {
        #region Private Members

        private readonly NavigationStore _navigationStore;
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly IServiceProvider _serviceProvider;
        private const string PAUSE_IMAGE_PATH = @"/Icons/pause.png";
        private const string PLAY_IMAGE_PATH = @"/Icons/play.png";

        #endregion

        #region Properties

        public BaseViewModel CurrentViewModel => _navigationStore.CurrentViewModel;
        public BaseViewModel? CurrentModalViewModel => _modalNavigationStore.CurrentViewModel;
        public NavigationBarViewModel NavigationBarViewModel { get; }
        public MusicPlayerViewModel MusicPlayerViewModel { get; }
        public bool IsModalOpen => _modalNavigationStore.IsOpen;

        private bool _isMainEnabled = true;
        public bool IsMainEnabled
        {
            get => _isMainEnabled;
            set 
            { 
                _isMainEnabled = value;
                OnPropertyChanged(nameof(IsMainEnabled));
            }
        }

        private string _playPauseImage = PLAY_IMAGE_PATH;
        public string PlayPauseImage 
        { 
            get => _playPauseImage;
            set
            {
                _playPauseImage = value;
                OnPropertyChanged(nameof(PlayPauseImage));
            }
        }

        private NowPlayingViewModel? _nowPlayingViewModel;
        public NowPlayingViewModel? NowPlayingViewModel
        {
            get => _nowPlayingViewModel;
            private set
            {
                _nowPlayingViewModel = value;
                OnPropertyChanged(nameof(NowPlayingViewModel));
            }
        }

        private bool _nowPlayingViewModelVisibility = false;
        public bool NowPlayingViewModelVisibility
        {
            get => _nowPlayingViewModelVisibility;
            set 
            { 
                _nowPlayingViewModelVisibility = value;
                OnPropertyChanged(nameof(NowPlayingViewModelVisibility));
            }
        }

        #endregion

        #region Commands 

        public ICommand PlayPauseCommand { get; private set; }
        public ICommand PlayNextCommand { get; private set; }
        public ICommand PlayPreviousCommand { get; private set; }

        #endregion

        public MainViewModel(IServiceProvider serviceProvider,
            ModalNavigationStore modalNavigationStore,
            NavigationStore navigationStore,
            MusicPlayerViewModel musicPlayerViewModel,
            NavigationBarViewModel navigationBarViewModel)
        {
            _serviceProvider = serviceProvider;
            _modalNavigationStore = modalNavigationStore;
            _navigationStore = navigationStore;

            MusicPlayerViewModel = musicPlayerViewModel;
            NavigationBarViewModel = navigationBarViewModel;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _modalNavigationStore.CurrentViewModelChanged += OnCurrentModalViewModelChanged;
            MusicPlayerViewModel.PlayingStateChanged += OnIsPlayingChanged;
            MusicPlayerViewModel.NavigateToNowPlaying += OnNavigateToNowPlaying;

            PlayPauseCommand = MusicPlayerViewModel.PlayPauseCommand;
            PlayNextCommand = MusicPlayerViewModel.PlayNextCommand;
            PlayPreviousCommand = MusicPlayerViewModel.PlayPreviousCommand;
        }

        #region Private Methods

        private void OnNavigateToNowPlaying()
        {
            MusicPlayerViewModel.IsEnabled = false;
            NavigationBarViewModel.IsEnabled = false; 
            IsMainEnabled = false;

            if (CurrentModalViewModel is not null)
            {
                _modalNavigationStore.CurrentViewModel = null;
            }

            NowPlayingViewModel = _serviceProvider.GetRequiredService<NowPlayingViewModel>();
            NowPlayingViewModel.ShowWindow = true;
        }

        private void OnIsPlayingChanged()
        {
            PlayPauseImage = MusicPlayerViewModel.IsPlaying ? PAUSE_IMAGE_PATH : PLAY_IMAGE_PATH;
        }

        private void OnCurrentModalViewModelChanged()
        {
            if (CurrentModalViewModel is null)
            {
                EnableControls();
            }
            else
            {
                NavigationBarViewModel.IsEnabled = false;
                IsMainEnabled = false;
            }

            OnPropertyChanged(nameof(CurrentModalViewModel));
            OnPropertyChanged(nameof(IsModalOpen));
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        #endregion

        #region Public Methods

        public void EnableControls()
        {
            MusicPlayerViewModel.IsEnabled = true;
            NavigationBarViewModel.IsEnabled = true;
            IsMainEnabled = true;
        }

        public void OnClosing()
        {
            var trackStore = _serviceProvider.GetRequiredService<TrackStore>();

            if (trackStore.PlaylistsChanged)
            {
                var playlistPersistenceService = _serviceProvider.GetRequiredService<PlaylistPersistenceService>();

                for (int i = 0; i < trackStore.AvailablePlaylists.Count; i++)
                {
                    var currentPlaylist = trackStore.AvailablePlaylists[i];

                    playlistPersistenceService.Add(currentPlaylist.Name, 
                        currentPlaylist.DateCreated.ToString(), 
                        currentPlaylist.ImagePath, 
                        currentPlaylist.Tracks.Select(t => t.FilePath).ToList());
                }

                playlistPersistenceService.Save();
            }

            Properties.Settings.Default.Volume = MusicPlayerViewModel.Volume;
            Properties.Settings.Default.Save();

            MusicPlayerViewModel.Dispose();
            CurrentViewModel.Dispose();
            CurrentModalViewModel?.Dispose();
        }

        #endregion Public Methods
    }
}

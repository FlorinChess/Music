using Music.WPF.Commands;
using Music.WPF.Services;
using Music.WPF.Store;
using System.Windows.Input;

namespace Music.WPF.ViewModels
{
    public sealed class NavigationBarViewModel : BaseViewModel
    {
        #region Private Members

        private readonly INavigationService _myMusicNavigationService;
        private readonly INavigationService _playlistCollectionNavigationService;
        private readonly INavigationService _settingsNavigationService;
        private readonly INavigationService _searchNavigationService;
        private readonly NavigationStore _navigationStore;
        private ICommand _navigateMyMusicCommand;
        private ICommand _navigatePlaylistCollectionCommand;
        private ICommand _navigateSettingsCommand;
        private ICommand _navigateSearchCommand;
        private ICommand _navigateBackCommand;

        #endregion

        #region Properties

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set 
            {
                if (_isEnabled == value) return;

                _isEnabled = value; 
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        private bool _navigateBackEnabled = false;
        public bool NavigateBackEnabled
        {
            get => _navigateBackEnabled;
            set
            {
                if (_navigateBackEnabled == value) return;

                _navigateBackEnabled = value;
                OnPropertyChanged(nameof(NavigateBackEnabled));
            }
        }

        #endregion Properties

        #region Commands

        public ICommand NavigateMyMusicCommand => _navigateMyMusicCommand ??= new NavigateCommand(_myMusicNavigationService);
        public ICommand NavigatePlaylistCollectionCommand => _navigatePlaylistCollectionCommand ??= new NavigateCommand(_playlistCollectionNavigationService);
        public ICommand NavigateSettingsCommand => _navigateSettingsCommand ??= new NavigateCommand(_settingsNavigationService);
        public ICommand NavigateSearchCommand => _navigateSearchCommand ??= new NavigateCommand(_searchNavigationService);
        public ICommand NavigateBackCommand => _navigateBackCommand ??= new NavigateBackCommand(_navigationStore);

        #endregion

        public NavigationBarViewModel(INavigationService myMusicNavigationService,
            INavigationService playlistCollectionNavigationService,
            INavigationService settingsNavigationService,
            INavigationService searchNavigationService,
            NavigationStore navigationStore)
        {
            _myMusicNavigationService = myMusicNavigationService;
            _playlistCollectionNavigationService = playlistCollectionNavigationService;
            _settingsNavigationService = settingsNavigationService;
            _searchNavigationService = searchNavigationService;
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += delegate { NavigateBackEnabled = _navigationStore.Count() > 1; };

            NavigateBackEnabled = _navigationStore.Count() > 1;
        }
    }
}

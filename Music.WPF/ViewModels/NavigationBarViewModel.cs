using Music.WPF.Commands;
using Music.WPF.Services;
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
        private ICommand _navigateMyMusicCommand;
        private ICommand _navigatePlaylistCollectionCommand;
        private ICommand _navigateSettingsCommand;
        private ICommand _navigateSearchCommand;

        #endregion

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set 
            { 
                _isEnabled = value; 
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        #region Commands

        public ICommand NavigateMyMusicCommand => _navigateMyMusicCommand ??= new NavigateCommand(_myMusicNavigationService);
        public ICommand NavigatePlaylistCollectionCommand => _navigatePlaylistCollectionCommand ??= new NavigateCommand(_playlistCollectionNavigationService);
        public ICommand NavigateSettingsCommand => _navigateSettingsCommand ??= new NavigateCommand(_settingsNavigationService);
        public ICommand NavigateSearchCommand => _navigateSearchCommand ??= new NavigateCommand(_searchNavigationService);

        #endregion

        public NavigationBarViewModel(INavigationService myMusicNavigationService,
            INavigationService playlistCollectionNavigationService,
            INavigationService settingsNavigationService,
            INavigationService searchNavigationService)
        {
            _myMusicNavigationService = myMusicNavigationService;
            _playlistCollectionNavigationService = playlistCollectionNavigationService;
            _settingsNavigationService = settingsNavigationService;
            _searchNavigationService = searchNavigationService;
        }
    }
}

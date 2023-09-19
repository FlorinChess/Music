using Music.WPF.Core;
using System.Windows.Input;

namespace Music.WPF.Models
{
    public class PlaylistItemModel : ObservableObject
    {
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

        public ICommand NavigateToSelectedPlaylistCommand { get; set; }

        public PlaylistItemModel(PlaylistModel playlist)
        {
            _playlist = playlist;
        }
    }
}

using Music.WPF.Core;
using Music.WPF.Models;

namespace Music.WPF.Modals.Models
{
    public sealed class PlaylistListViewItem : ObservableObject
    {
        private bool _hasTrack = false;
        public bool HasTrack
        {
            get => _hasTrack;
            set
            {
                _hasTrack = value;
                OnPropertyChanged(nameof(HasTrack));
            }
        }
        public PlaylistModel Playlist { get; set; }

        public PlaylistListViewItem(PlaylistModel playlist, TrackModel track)
        {
            HasTrack = playlist.Contains(track);
            Playlist = playlist;
        }
    }
}

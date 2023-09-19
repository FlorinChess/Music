using Music.WPF.Commands;
using Music.WPF.Modals.Models;
using Music.WPF.Models;
using Music.WPF.Store;
using Music.WPF.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Music.WPF.Modals.ViewModels
{
    public sealed class AddToPlaylistModalViewModel : BaseViewModel, IModal
    {
        #region Properties

        public ObservableCollection<PlaylistListViewItem> Playlists { get; set; } = new();
        public TrackModel SelectedTrack { get; }

        #endregion

        #region Commands

        public CloseModalCommand CloseModalCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        #endregion

        public AddToPlaylistModalViewModel(ModalNavigationStore modalNavigationStore, TrackStore trackStore, TrackModel selectedTrack)
        {
            CloseModalCommand = modalNavigationStore.CloseModalCommand;
            SaveCommand = new RelayCommand(_ => Save());

            CreateListViewItems(trackStore.AvailablePlaylists);

            CheckIfTrackIsInPlaylists(selectedTrack);

            SelectedTrack = selectedTrack;
        }

        private void CheckIfTrackIsInPlaylists(TrackModel selectedTrack)
        {
            for (int i = 0; i < Playlists.Count; i++)
            {
                PlaylistListViewItem item = Playlists[i];

                if (item.Playlist.Contains(selectedTrack))
                {
                    item.HasTrack = true;
                }
            }
        }

        #region Private Methods

        private void CreateListViewItems(IList<PlaylistModel> playlists)
        {
            for (int i = 0; i < playlists.Count; i++)
            {
                Playlists.Add(new PlaylistListViewItem(playlists[i], SelectedTrack));
            }
        }

        private void Save()
        {
            AddTrackToPlaylists(SelectedTrack, Playlists);

            CloseModalCommand.Execute(null);
        }

        private static void AddTrackToPlaylists(TrackModel track, Collection<PlaylistListViewItem> playlistItems)
        {
            for (int i = 0; i < playlistItems.Count; i++)
            {
                if (!playlistItems[i].HasTrack || playlistItems[i].Playlist.Contains(track)) continue;

                playlistItems[i].Playlist.Tracks.Add(track);
            }
        }

        #endregion
    }
}

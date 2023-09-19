using Microsoft.Win32;
using Music.WPF.Commands;
using Music.WPF.Models;
using Music.WPF.Store;
using Music.WPF.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Music.WPF.Modals.ViewModels
{
    public sealed class EditPlaylistModalViewModel : BaseViewModel, IModal
    {
        private readonly TrackStore _trackStore;

        #region Properties

        public ObservableCollection<string> PlaylistTags { get; set; }
        public PlaylistModel OriginalPlaylist { get; private set; }
        public PlaylistModel SelectedPlaylist { get; set; }
        public string PlaylistName { get; set; }

        private bool _isRemoveButtonVisible;
        public bool IsRemoveButtonVisible
        {
            get => _isRemoveButtonVisible;
            set 
            { 
                _isRemoveButtonVisible = value;
                OnPropertyChanged(nameof(IsRemoveButtonVisible));
            }
        }

        private string _playlistImagePath;
        public string PlaylistImagePath 
        { 
            get => _playlistImagePath;
            set
            {
                _playlistImagePath = value;
                OnPropertyChanged(nameof(PlaylistImagePath));
            }
        }

        #endregion Properties

        #region Commands

        public CloseModalCommand CloseModalCommand { get; set; }
        public ICommand RemovePlaylistImage { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SelectPlaylistImage { get; set; }

        #endregion

        public EditPlaylistModalViewModel(ModalNavigationStore modalNavigationStore, TrackStore trackStore, PlaylistModel playlist)
        {
            _trackStore = trackStore;
            OriginalPlaylist = playlist;

            CloseModalCommand = modalNavigationStore.CloseModalCommand;
            RemovePlaylistImage = new RelayCommand(_ => RemoveImage());
            SaveCommand = new RelayCommand(_ =>
            {
                Save();

                modalNavigationStore.Close();
            });
            SelectPlaylistImage = new RelayCommand(_ => SelectImage());

            // Clone the selected playlist and bind the clone to the UI; on save, replicate changes on the original object
            SelectedPlaylist = (PlaylistModel) playlist.Clone();

            IsRemoveButtonVisible = SelectedPlaylist.ImagePath != string.Empty;

            SetInitialValues();
        }

        #region Private Methods

        private void RemoveImage()
        {
            PlaylistImagePath = string.Empty;
            IsRemoveButtonVisible = false;
        }

        private void SelectImage()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Title = "Select image...",
                Filter = "Image Files|*.png;*.jpg;*.jpeg",
                CheckFileExists = true,
                RestoreDirectory = true,
                InitialDirectory = Environment.SpecialFolder.MyPictures.ToString()
            };

            if (openFileDialog.ShowDialog() == true)
            {
                PlaylistImagePath = openFileDialog.FileName;
                IsRemoveButtonVisible = true;
            }
        }

        private void Save()
        {
            OriginalPlaylist.Name = PlaylistName;
            OriginalPlaylist.ImagePath = PlaylistImagePath;

            _trackStore.PlaylistUpdated();
        }

        private void SetInitialValues()
        {
            PlaylistName = SelectedPlaylist.Name;
            PlaylistImagePath = SelectedPlaylist.ImagePath;

            OnPropertyChanged(nameof(PlaylistName));
        }

        #endregion
    }
}

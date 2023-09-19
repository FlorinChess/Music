using Microsoft.Extensions.DependencyInjection;
using Music.APIs;
using Music.NAudio;
using Music.WPF.Commands;
using Music.WPF.Models;
using Music.WPF.Services;
using Music.WPF.Store;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Music.WPF.ViewModels
{
    public sealed class SettingsViewModel : BaseViewModel
    {
        #region Private Members

        private readonly ResourceDictionary _resourceDictionary = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly MusicMetadataService _musicMetadataService;
        private readonly BackgroundWorker _worker = new();
        private Uri _dictionarySource;

        #endregion

        #region Properties

        public ObservableCollection<MusicFolderModel> MusicFolders { get; set; } = new ObservableCollection<MusicFolderModel>();
        public ObservableCollection<string> OutputDevices { get; set; } = new ObservableCollection<string>();
        public MusicFolderModel SelectedMusicFolder { get; set; }

        private bool _isAutoPlayEnabled = Properties.Settings.Default.AutoplayEnabled;
        public bool IsAutoPlayEnabled
        {
            get => _isAutoPlayEnabled;
            set
            {
                _isAutoPlayEnabled = value;
                OnPropertyChanged(nameof(IsAutoPlayEnabled));
            }
        }

        private bool _isMetadataAutocompleteEnabled = Properties.Settings.Default.MetadataAutocompleteEnabled;
        public bool IsMetadataAutocompleteEnabled
        {
            get => _isMetadataAutocompleteEnabled;
            set
            {
                _isMetadataAutocompleteEnabled = value;
                OnPropertyChanged(nameof(IsMetadataAutocompleteEnabled));
            }
        }

        private string _currentOutputDevice = MusicPlayer.GetDefaultOutputDevice().FriendlyName;
        public string CurrentOutputDevice
        {
            get => _currentOutputDevice;
            set
            {
                if (_currentOutputDevice == value) return;

                _currentOutputDevice = value;
                OnPropertyChanged(nameof(CurrentOutputDevice));
            }
        }

        #endregion

        #region Commands

        public ICommand ChangeThemeCommand { get; }
        public ICommand OpenEqualizerCommand { get; set; }
        public ICommand RemoveMusicFolderCommand { get; }
        public ICommand SelectMusicFilesFolderCommand { get; }


        #endregion

        public SettingsViewModel(IServiceProvider serviceProvider, TrackStore trackStore, MusicMetadataService musicMetadataService)
        {
            _serviceProvider = serviceProvider;
            _musicMetadataService = musicMetadataService;

            _worker.DoWork += OnBackgroundWorkerDoWork;

            AddMusicFolders();
            AddOutputDevices();

            SelectMusicFilesFolderCommand = new RelayCommand(_ => SelectMusicFolder());
            ChangeThemeCommand = new RelayCommand((color) => ChangeTheme(color!));
            RemoveMusicFolderCommand = new RelayCommand(_ => RemoveMusicFolder());
            OpenEqualizerCommand = _serviceProvider.GetRequiredService<MusicPlayerViewModel>().OpenEqualizerCommand;

            if (IsMetadataAutocompleteEnabled == true)
            {
                _musicMetadataService.AddMusicFiles(trackStore.AvailableTracks.Select(t => t.FilePath));
            }
        }

        #region Private Methods

        private void AddMusicFolders()
        {
            var musicFolders = MusicFilesService.GetMusicFolders();

            for (int i = 0; i < musicFolders.Count();i++)
                MusicFolders.Add(new MusicFolderModel { Path = musicFolders.ElementAt(i) });
        }

        private void RemoveMusicFolder()
        {
            try
            {
                if (SelectedMusicFolder is null) return;

                Properties.Settings.Default.MusicFolder.Remove(SelectedMusicFolder.Path);
                Properties.Settings.Default.Save();

                MusicFolders.Remove(SelectedMusicFolder);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void SelectMusicFolder()
        {
            var newFolder = MusicFilesService.AddNewMusicFolder();

            if (!string.IsNullOrEmpty(newFolder))
            {
                // Check if the new folder is already contained in the music folders
                for (int i = 0; i < MusicFolders.Count; i++)
                    if (newFolder == MusicFolders[i].Path) return;

                MusicFolders.Add(new MusicFolderModel { Path = newFolder });
                Properties.Settings.Default.MusicFolder.Add(newFolder);
                Properties.Settings.Default.Save();
            }
        }

        private async void OnBackgroundWorkerDoWork(object? sender, DoWorkEventArgs e)
        {
            await _musicMetadataService.FillMetadataAsync();
        }

        private void ChangeTheme(object color)
        {
            try
            {
                switch ((string)color)
                {
                    case "Red":
                        _dictionarySource = new Uri("/Music.WPF;component/Themes/ColorThemes/RedTheme.xaml", UriKind.RelativeOrAbsolute);
                        _resourceDictionary.Source = _dictionarySource;
                        break;
                    case "Blue":
                        _dictionarySource = new Uri("/Music.WPF;component/Themes/ColorThemes/BlueTheme.xaml", UriKind.RelativeOrAbsolute);
                        _resourceDictionary.Source = _dictionarySource;
                        break;
                    case "Green":
                        _dictionarySource = new Uri("/Music.WPF;component/Themes/ColorThemes/GreenTheme.xaml", UriKind.RelativeOrAbsolute);
                        _resourceDictionary.Source = _dictionarySource;
                        break;
                    case "Yellow":
                        _dictionarySource = new Uri("/Music.WPF;component/Themes/ColorThemes/YellowTheme.xaml", UriKind.RelativeOrAbsolute);
                        _resourceDictionary.Source = _dictionarySource;
                        break;
                    case "Purple":
                        _dictionarySource = new Uri("/Music.WPF;component/Themes/ColorThemes/PurpleTheme.xaml", UriKind.RelativeOrAbsolute);
                        _resourceDictionary.Source = _dictionarySource;
                        break;
                    default:
                        _dictionarySource = new Uri("/Music.WPF;component/Themes/ColorThemes/YellowTheme.xaml", UriKind.RelativeOrAbsolute);
                        _resourceDictionary.Source = _dictionarySource;
                        break;
                }

                if (Application.Current.Resources.MergedDictionaries[0].Source == _resourceDictionary.Source) return;

                Application.Current.Resources.MergedDictionaries[0] = _resourceDictionary;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void AddOutputDevices()
        {
            var devices = MusicPlayer.GetOutputDevices();

            foreach (var device in devices)
            {
                OutputDevices.Add(device.Value);
            }
        }

        #endregion

        public override void Dispose()
        {
            Properties.Settings.Default.AutoplayEnabled = IsAutoPlayEnabled;
            Properties.Settings.Default.MetadataAutocompleteEnabled = IsMetadataAutocompleteEnabled;
            Properties.Settings.Default.Save();
            base.Dispose();
        }
    }
}

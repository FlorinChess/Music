using Music.NAudio;
using Music.WPF.Commands;
using Music.WPF.Modals.Models;
using Music.WPF.Store;
using Music.WPF.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Music.WPF.Modals.ViewModels
{
    public sealed class EqualizerModalViewModel : BaseViewModel, IModal
    {
        private readonly MusicPlayer _musicPlayer;

        #region Properties

        public ObservableCollection<EqualizerProfile> EqualizerProfiles { get; private set; } = new ObservableCollection<EqualizerProfile>();

        private string _equalizerStatusText;
        public string EqualizerStatusText
        {
            get => _equalizerStatusText;
            set
            {
                _equalizerStatusText = value;
                OnPropertyChanged(nameof(EqualizerStatusText));
            }
        }

        private bool _isEqualizerEnabled = false;
        public bool IsEqualizerEnabled
        {
            get => _isEqualizerEnabled;
            set 
            { 
                _isEqualizerEnabled = value;
                OnPropertyChanged(nameof(IsEqualizerEnabled));

                EqualizerStatusText = IsEqualizerEnabled ? "On" : "Off";
            }
        }

        public float Band1
        {
            get => MusicPlayer.Bands[0].Gain; 
            set 
            {
                if (MusicPlayer.Bands[0].Gain == value) return;

                MusicPlayer.Bands[0].Gain = value;
                OnPropertyChanged(nameof(Band1));
            }
        }

        public float Band2
        {
            get => MusicPlayer.Bands[1].Gain;
            set
            {
                if (MusicPlayer.Bands[1].Gain == value) return;

                MusicPlayer.Bands[1].Gain = value;
                OnPropertyChanged(nameof(Band2));
            }
        }

        public float Band3
        {
            get => MusicPlayer.Bands[2].Gain;
            set
            {
                if (MusicPlayer.Bands[2].Gain == value) return;

                MusicPlayer.Bands[2].Gain = value;
                OnPropertyChanged(nameof(Band3));
            }
        }

        public float Band4
        {
            get => MusicPlayer.Bands[3].Gain;
            set
            {
                if (MusicPlayer.Bands[3].Gain == value) return;

                MusicPlayer.Bands[3].Gain = value;
                OnPropertyChanged(nameof(Band4));
            }
        }

        public float Band5
        {
            get => MusicPlayer.Bands[4].Gain;
            set
            {
                if (MusicPlayer.Bands[4].Gain == value) return;

                MusicPlayer.Bands[4].Gain = value;
                OnPropertyChanged(nameof(Band5));
            }
        }

        public float Band6
        {
            get => MusicPlayer.Bands[5].Gain;
            set
            {
                if (MusicPlayer.Bands[5].Gain == value) return;

                MusicPlayer.Bands[5].Gain = value;
                OnPropertyChanged(nameof(Band6));
            }
        }

        public float Band7
        {
            get => MusicPlayer.Bands[6].Gain;
            set
            {
                if (MusicPlayer.Bands[6].Gain == value) return;

                MusicPlayer.Bands[6].Gain = value;
                OnPropertyChanged(nameof(Band7));
            }
        }

        public float Band8
        {
            get => MusicPlayer.Bands[7].Gain;
            set
            {
                if (MusicPlayer.Bands[7].Gain == value) return;

                MusicPlayer.Bands[7].Gain = value;
                OnPropertyChanged(nameof(Band8));
            }
        }

        public float Band9
        {
            get => MusicPlayer.Bands[8].Gain;
            set
            {
                if (MusicPlayer.Bands[8].Gain == value) return;

                MusicPlayer.Bands[8].Gain = value;
                OnPropertyChanged(nameof(Band9));
            }
        }

        public float Band10
        {
            get => MusicPlayer.Bands[9].Gain;
            set
            {
                if (MusicPlayer.Bands[9].Gain == value) return;

                MusicPlayer.Bands[9].Gain = value;
                OnPropertyChanged(nameof(Band10));
            }
        }

        public static float Maximum => 20f;
        public static float Minimum => -20f;

        private EqualizerProfile _selectedEqualizerProfile;
        public EqualizerProfile SelectedEqualizerProfile
        {
            get => _selectedEqualizerProfile;
            set 
            { 
                if (_selectedEqualizerProfile == value) return;

                _selectedEqualizerProfile = value;
                ApplyEqualizerProfile(_selectedEqualizerProfile);
                OnPropertyChanged(nameof(SelectedEqualizerProfile));
            }
        }


        #endregion Properties

        #region Commands

        public CloseModalCommand CloseModalCommand { get; set; }
        public ICommand ResetEqualizerCommand { get; private set; }
        public ICommand SaveCommand { get; set; }

        #endregion

        public EqualizerModalViewModel(ModalNavigationStore modalNavigationStore, MusicPlayer musicPlayer)
        {
            CloseModalCommand = modalNavigationStore.CloseModalCommand;
            _musicPlayer = musicPlayer;


            ResetEqualizerCommand = new RelayCommand(_ => ResetEqualizer());
            SaveCommand = new RelayCommand(_ => 
            {
                Save();

                CloseModalCommand.Execute(null);
            });

            //SelectedEqualizerProfile = new EqualizerProfile("Default", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);

            EqualizerStatusText = IsEqualizerEnabled ? "On" : "Off";
        }

        #region Private Methods

        private void Save()
        {
            
        }

        private void LoadEqualizerProfiles()
        {
        }

        private void ApplyEqualizerProfile(EqualizerProfile equalizerProfile)
        {
            Band1 = equalizerProfile.Band1;
            Band2 = equalizerProfile.Band2;
            Band3 = equalizerProfile.Band3;
            Band4 = equalizerProfile.Band4;
            Band5 = equalizerProfile.Band5;
            Band6 = equalizerProfile.Band6;
            Band7 = equalizerProfile.Band7;
            Band8 = equalizerProfile.Band8;
            Band9 = equalizerProfile.Band9;
            Band10 = equalizerProfile.Band10;
        }

        private void ResetEqualizer()
        {
            Band1 = 0f;
            Band2 = 0f;
            Band3 = 0f;
            Band4 = 0f;
            Band5 = 0f;
            Band6 = 0f;
            Band7 = 0f;
            Band8 = 0f;
            Band9 = 0f;
            Band10 = 0f;
        }

        #endregion Private Methods

        public override void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            _musicPlayer?.UpdateEqualizer();
        }

    }
}

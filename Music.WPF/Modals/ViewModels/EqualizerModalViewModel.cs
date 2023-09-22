using Music.NAudio;
using Music.WPF.Commands;
using Music.WPF.Modals.Models;
using Music.WPF.Store;
using Music.WPF.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Music.WPF.Modals.ViewModels
{
    public sealed class EqualizerModalViewModel : BaseViewModel, IModal
    {
        private readonly MusicPlayer _musicPlayer;

        #region Properties

        public ObservableCollection<EqualizerProfile> EqualizerProfiles { get; private set; } = new ObservableCollection<EqualizerProfile>();
        public bool EqualizerChanged { get; private set; } = false;

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

        private EqualizerProfile _customEqualizerProfile;
        public EqualizerProfile CustomEqualizerProfile
        {
            get => _customEqualizerProfile;
            set
            {
                _customEqualizerProfile = value;
                OnPropertyChanged(nameof(CustomEqualizerProfile));
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
                _selectedEqualizerProfile = value;

                if (_selectedEqualizerProfile is not null && !EqualizerChanged)
                    ApplyEqualizerProfile(_selectedEqualizerProfile);

                OnPropertyChanged(nameof(SelectedEqualizerProfile));

                PrinAll();
            }
        }

        private void PrinAll()
        {
            foreach(var e in EqualizerProfiles)
            {
                Debug.Write(e.Name + ", ");
                Debug.Write(e.Band1 + ", ");
                Debug.Write(e.Band2 + ", ");
                Debug.Write(e.Band3 + ", ");
                Debug.Write(e.Band4 + ", ");
                Debug.Write(e.Band5 + ", ");
                Debug.Write(e.Band6 + ", ");
                Debug.Write(e.Band7 + ", ");
                Debug.Write(e.Band8 + ", ");
                Debug.Write(e.Band9 + ", ");
                Debug.WriteLine(e.Band10);
            }
                Debug.WriteLine("");
        }

        #endregion Properties

        #region Commands

        public CloseModalCommand CloseModalCommand { get; set; }
        public ICommand ResetEqualizerCommand { get; private set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DragStartedCommand { get; private set; }
        public ICommand DragCompletedCommand { get; private set; }

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

            DragStartedCommand = new RelayCommand(_ =>
            {
                if (EqualizerChanged) return;

                EqualizerChanged = true;

                if (SelectedEqualizerProfile.Name != "Manual")
                {
                    EqualizerProfiles[3] = EqualizerProfiles[3] with { Name = "Manual"};
                    SelectedEqualizerProfile = EqualizerProfiles[3];
                }
            });

            DragCompletedCommand = new RelayCommand(_ => 
            {
                EqualizerProfiles[3] = EqualizerProfiles[3] with 
                { 
                    Band1 = Band1,
                    Band2 = Band2,
                    Band3 = Band3,
                    Band4 = Band4,
                    Band5 = Band5,
                    Band6 = Band6,
                    Band7 = Band7,
                    Band8 = Band8,
                    Band9 = Band9,
                    Band10 = Band10,
                };
                SelectedEqualizerProfile = EqualizerProfiles[3];

                EqualizerChanged = false;
            });

            CreateEqualizerProfiles();
            SelectedEqualizerProfile = EqualizerProfiles[0];

            EqualizerStatusText = IsEqualizerEnabled ? "On" : "Off";
        }

        #region Private Methods

        private void Save()
        {
            
        }

        private void CreateEqualizerProfiles()
        {
            EqualizerProfiles.Add(new EqualizerProfile("Flat", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f)); 
            EqualizerProfiles.Add(new EqualizerProfile("Bass boost", 4f, 7f, 4.8f, 4f, 1f, 0f, 0f, 0f, 0f, 0f));
            EqualizerProfiles.Add(new EqualizerProfile("Treble boost", 0f, 0f, 0f, 0f, 0f, 0f, 1f, 4f, 4.8f, 7f));
            EqualizerProfiles.Add(new EqualizerProfile("Manual", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f));
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

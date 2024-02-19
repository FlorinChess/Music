using Music.NAudio;
using Music.WPF.Commands;
using Music.WPF.Modals.Models;
using Music.WPF.Store;
using Music.WPF.ViewModels;
using System.Collections.ObjectModel;
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
            }
        }

        #endregion Properties

        #region Commands

        public CloseModalCommand CloseModalCommand { get; set; }
        public ICommand DragCompletedCommand { get; private set; }
        public ICommand DragStartedCommand { get; private set; }
        public ICommand ResetEqualizerCommand { get; private set; }
        public ICommand SaveCommand { get; set; }

        #endregion

        public EqualizerModalViewModel(ModalNavigationStore modalNavigationStore, MusicPlayer musicPlayer)
        {
            _musicPlayer = musicPlayer;

            CloseModalCommand = modalNavigationStore.CloseModalCommand;
            DragCompletedCommand = new RelayCommand(_ => OnDragCompleted());
            DragStartedCommand = new RelayCommand(_ => OnDragStarted());
            ResetEqualizerCommand = new RelayCommand(_ => ResetEqualizer());
            SaveCommand = new RelayCommand(_ => Save());

            CreateEqualizerProfiles();

            EqualizerStatusText = IsEqualizerEnabled ? "On" : "Off";
        }

        #region Private Methods

        private void OnDragStarted()
        {
            if (EqualizerChanged) return;

            EqualizerChanged = true;

            if (SelectedEqualizerProfile.Name != "Manual")
            {
                EqualizerProfiles[3] = SelectedEqualizerProfile with { Name = "Manual" };
                SelectedEqualizerProfile = EqualizerProfiles[3];
            }
        }

        private void OnDragCompleted()
        {
            // This creates a new instance of EqualizerProfile with the specified property changes
            // NOTE: the name property preservs its initial value ('Manual')
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
        }

        private void Save()
        {
            SaveSettings();

            CloseModalCommand.Execute(null);
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.EqualizerProfile = SelectedEqualizerProfile.Name;

            // Save manual profile 
            Properties.Settings.Default.Band1 = EqualizerProfiles[3].Band1;
            Properties.Settings.Default.Band2 = EqualizerProfiles[3].Band2;
            Properties.Settings.Default.Band3 = EqualizerProfiles[3].Band3;
            Properties.Settings.Default.Band4 = EqualizerProfiles[3].Band4;
            Properties.Settings.Default.Band5 = EqualizerProfiles[3].Band5;
            Properties.Settings.Default.Band6 = EqualizerProfiles[3].Band6;
            Properties.Settings.Default.Band7 = EqualizerProfiles[3].Band7;
            Properties.Settings.Default.Band8 = EqualizerProfiles[3].Band8;
            Properties.Settings.Default.Band9 = EqualizerProfiles[3].Band9;
            Properties.Settings.Default.Band10 = EqualizerProfiles[3].Band10;

            Properties.Settings.Default.Save();
        }

        private static EqualizerProfile CreateManualEqualizerProfile()
        {
            var b1 = Properties.Settings.Default.Band1;
            var b2 = Properties.Settings.Default.Band2;
            var b3 = Properties.Settings.Default.Band3;
            var b4 = Properties.Settings.Default.Band4;
            var b5 = Properties.Settings.Default.Band5;
            var b6 = Properties.Settings.Default.Band6;
            var b7 = Properties.Settings.Default.Band7;
            var b8 = Properties.Settings.Default.Band8;
            var b9 = Properties.Settings.Default.Band9;
            var b10 = Properties.Settings.Default.Band10;

            return new EqualizerProfile("Manual", b1, b2, b3, b4, b5, b6, b7, b8, b9, b10);
        }

        private void CreateEqualizerProfiles()
        {
            EqualizerProfiles.Add(new EqualizerProfile("Flat", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f)); 
            EqualizerProfiles.Add(new EqualizerProfile("Bass boost", 4f, 7f, 4.8f, 4f, 1f, 0f, 0f, 0f, 0f, 0f));
            EqualizerProfiles.Add(new EqualizerProfile("Treble boost", 0f, 0f, 0f, 0f, 0f, 0f, 1f, 4f, 4.8f, 7f));
            EqualizerProfiles.Add(CreateManualEqualizerProfile());

            for (int i = 0; i < EqualizerProfiles.Count; i++)
            {
                if (EqualizerProfiles[i].Name == Properties.Settings.Default.EqualizerProfile)
                {
                    SelectedEqualizerProfile = EqualizerProfiles[i];
                    break;
                }
            }
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
            SelectedEqualizerProfile = EqualizerProfiles[0];
        }

        #endregion Private Methods

        public override void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            _musicPlayer?.UpdateEqualizer();
        }

    }
}

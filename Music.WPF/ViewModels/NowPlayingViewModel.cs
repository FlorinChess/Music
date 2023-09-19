using Music.NAudio.WaveformGenerator;
using Music.WPF.Commands;
using Music.WPF.Models;
using Music.WPF.Store;
using System.Windows;
using System.Windows.Input;

namespace Music.WPF.ViewModels
{
    public sealed class NowPlayingViewModel : BaseViewModel
    {
        #region Private Members

        private readonly TrackStore _trackStore;
        private readonly WaveformGenerator _waveformGenerator;

        #endregion

        #region Properties
        public TrackListComponentViewModel TrackListComponentViewModel { get; }
        public MusicPlayerViewModel MusicPlayerViewModel { get; }

        private bool _showWindow;
        public bool ShowWindow
        {
            get => _showWindow;
            set
            {
                _showWindow = value;
                OnPropertyChanged(nameof(ShowWindow));
            }
        }

        private string _reorderToolTip = "Reorder: off";
        public string ReorderToolTip
        {
            get => _reorderToolTip;
            set 
            { 
                _reorderToolTip = value;


                OnPropertyChanged(nameof(ReorderToolTip));
            }
        }

        private TrackModel _currentTrack;
        public TrackModel CurrentTrack
        {
            get => _currentTrack;
            set
            {
                _currentTrack = value;
                OnPropertyChanged(nameof(CurrentTrack));
            }
        }

        private float[] _waveformData;
        public float[] WaveformData
        {
            get => _waveformData;
            set
            {
                _waveformData = value;
                OnPropertyChanged(nameof(WaveformData));
            }
        }

        #endregion

        #region Commands

        public ICommand PlayCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand ReorderCommand { get; private set; }

        #endregion

        public NowPlayingViewModel(TrackStore trackStore, MainViewModel mainViewModel, MusicPlayerViewModel musicPlayerViewModel, TrackListComponentViewModel trackListComponentViewModel, WaveformGenerator waveformGenerator)
        {
            _trackStore = trackStore;
            _trackStore.CurrentTrackChanged += OnCurrentTrackChanged;

            MusicPlayerViewModel = musicPlayerViewModel;
            TrackListComponentViewModel = trackListComponentViewModel;

            _waveformGenerator = waveformGenerator;
            _waveformGenerator.WaveformDataUpdated += OnWaveformDataUpdated;

            // Order matters
            CurrentTrack = MusicPlayerViewModel.CurrentTrack!;

            WaveformData = _waveformGenerator.WaveformData;
            _waveformGenerator.GenerateWaveformData(CurrentTrack!.FilePath);
            // end

            CloseCommand = new RelayCommand(_ =>
            {
                ShowWindow = false;

                mainViewModel.EnableControls();
            });

            ReorderCommand = new RelayCommand(_ =>
            {
                ReorderToolTip = TrackListComponentViewModel.IsReorderEnabled ? "Reorder: On" : "Reorder: Off";
            });
        }

        private void OnWaveformDataUpdated(object? sender, WaveformDataEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                WaveformData = e.WaveformData;
            });
        }

        private void OnCurrentTrackChanged()
        {
            if (MusicPlayerViewModel.CurrentTrack is null) return;

            CurrentTrack = MusicPlayerViewModel.CurrentTrack;
            _waveformGenerator.GenerateWaveformData(CurrentTrack.FilePath);
        }

        public override void Dispose()
        {
            _trackStore.CurrentTrackChanged -= OnCurrentTrackChanged;
            _waveformGenerator.WaveformDataUpdated -= OnWaveformDataUpdated;

            base.Dispose();
        }
    }
}

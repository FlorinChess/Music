using Music.WPF.Commands;
using Music.WPF.Extensions;
using Music.WPF.Models;
using Music.WPF.Store;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Music.WPF.ViewModels
{
    public sealed class TrackListComponentViewModel : BaseViewModel
    {
        #region Private Members

        private readonly TrackStore _trackStore;

        #endregion

        #region Properties

        public ObservableCollection<TrackModel> Tracks { set; private get; } = new();

        private TrackModel _incomingItem;
        public TrackModel IncomingItem
        {
            get => _incomingItem;
            set
            {
                _incomingItem = value;
                OnPropertyChanged();
            }
        }

        private TrackModel _removedItem;
        public TrackModel RemovedItem
        {
            get => _removedItem;
            set
            {
                _removedItem = value;
                OnPropertyChanged();
            }
        }

        private TrackModel _targetItem;
        public TrackModel TargetItem
        {
            get => _targetItem;
            set
            {
                _targetItem = value;
                OnPropertyChanged();
            }
        }

        private TrackModel _insertedItem;
        public TrackModel InsertedItem
        {
            get => _insertedItem;
            set
            {
                _insertedItem = value;
                OnPropertyChanged();
            }
        }

        private TrackModel _selectedTrack;
        public TrackModel SelectedTrack
        {
            get => _selectedTrack;
            set { _selectedTrack = value; }
        }

        private bool _isReorderEnabled;
        public bool IsReorderEnabled
        {
            get => _isReorderEnabled; 
            set 
            { 
                _isReorderEnabled = value; 
                OnPropertyChanged(nameof(IsReorderEnabled));
            }
        }

        public int Count => Tracks.Count;


        #endregion

        #region Commands

        public ICommand ReceiveItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand InsertItemCommand { get; }
        public ICommand PlayCommand { get; set; }


        #endregion

        public TrackListComponentViewModel(TrackStore trackStore)
        {
            _trackStore = trackStore;
            _trackStore.QueueChanged += OnQueueChanged;

            Tracks.AddRange(_trackStore.Queue);

            ReceiveItemCommand = new ReceiveListViewItemCommand(this);
            RemoveItemCommand = new RemoveListViewItemCommand(this);
            InsertItemCommand = new InsertListViewItemCommand(this);
            PlayCommand = new RelayCommand(_ => PlaySelectedTrack());
        }

        #region Private Methods

        private void PlaySelectedTrack()
        {
            if (_selectedTrack is null) return;

            _trackStore.SetQueue(_selectedTrack);
        }

        private void OnQueueChanged()
        {
            Tracks.Clear();

            Tracks.AddRange(_trackStore.Queue);
        }

        #endregion Private Methods

        #region Internal Methods

        internal void AddNewItem(TrackModel incomingItem)
        {
            if (Tracks.Contains(incomingItem)) return;

            Tracks.Add(incomingItem);
        }

        internal void RemoveItem(TrackModel removedItem)
        {
            Tracks.Remove(removedItem);
        }

        internal void InsertItem(TrackModel insertedItem, TrackModel targetItem)
        {
            if (insertedItem == targetItem) return;

            int oldIndex = Tracks.IndexOf(insertedItem);
            int nextIndex = Tracks.IndexOf(targetItem);

            if (oldIndex == -1 || nextIndex == -1) return;

            Tracks.Move(oldIndex, nextIndex);

            _trackStore.SetQueue(Tracks, false);
        }

        #endregion

        public override void Dispose()
        {
            _trackStore.QueueChanged -= OnQueueChanged;
            base.Dispose();
        }
    }
}

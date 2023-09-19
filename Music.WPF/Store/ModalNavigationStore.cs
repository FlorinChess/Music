using Microsoft.Extensions.DependencyInjection;
using Music.WPF.Commands;
using Music.WPF.ViewModels;
using System;

namespace Music.WPF.Store
{
    public sealed class ModalNavigationStore
    {
        public event Action CurrentViewModelChanged;

        private BaseViewModel? _currentViewModel;
        public BaseViewModel? CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel?.Dispose();
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        public bool IsOpen => CurrentViewModel is not null;

        public CloseModalCommand CloseModalCommand { get; private set; }

        public ModalNavigationStore()
        {
            CloseModalCommand = new(this);
        }

        public void Close()
        {
            CurrentViewModel = null;
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}

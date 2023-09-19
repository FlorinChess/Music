using Music.WPF.ViewModels;
using System;
using System.Collections.Generic;

namespace Music.WPF.Store
{
    public sealed class NavigationStore
    {
        public event Action CurrentViewModelChanged;

        private readonly Stack<Type> _navigationStack = new();

        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel?.Dispose();
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }

        public void Push(Type viewModelType)
        {
            _navigationStack.Push(viewModelType);
        }

        public void Pop()
        {
            try 
            { 
                _navigationStack.Pop();
            } 
            catch { }
        }

        public int Count()
        {
            return _navigationStack.Count;
        }
    }
}

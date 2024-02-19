using Microsoft.Extensions.DependencyInjection;
using Music.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Music.WPF.Store
{
    public sealed class NavigationStore
    {
        public event Action CurrentViewModelChanged;

        #region Private Members

        private readonly Stack<PageIndex> _navigationStack = new();
        private readonly IServiceProvider _serviceProvider;
        private BaseViewModel _currentViewModel;

        #endregion Private Members

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel?.Dispose();
                _currentViewModel = value;

                if (_currentViewModel.GetPageIndex() != PageIndex.None)
                {
                    Push(_currentViewModel.GetPageIndex());
                }

                OnCurrentViewModelChanged();
            }
        }

        public NavigationStore(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }

        #region Public Methods

        public void Pop()
        {
            try 
            {
                if (_navigationStack.Count == 1) return;

                _navigationStack.Pop();
                _currentViewModel?.Dispose();

                switch(_navigationStack.Peek())
                {
                    case PageIndex.MyMusic:
                        _currentViewModel = _serviceProvider.GetRequiredService<MyMusicViewModel>();
                        break;
                    case PageIndex.PlaylistCollection:
                        _currentViewModel = _serviceProvider.GetRequiredService<PlaylistCollectionViewModel>();
                        break;
                    case PageIndex.Settings:
                        _currentViewModel = _serviceProvider.GetRequiredService<SettingsViewModel>();
                        break;
                    case PageIndex.Search:
                        _currentViewModel = _serviceProvider.GetRequiredService<SearchViewModel>();
                        break;
                }
                
                OnCurrentViewModelChanged();
            } 
            catch (Exception ex) 
            { 
                Debug.WriteLine(ex);
            }
        }

        public void Push(PageIndex viewModelType) => _navigationStack.Push(viewModelType);

        public int Count() => _navigationStack.Count;

        #endregion Public Methods
    }

    public enum PageIndex
    {
        None,
        MyMusic,
        Search,
        PlaylistCollection,
        Settings
    }
}

using Microsoft.Extensions.DependencyInjection;
using Music.WPF.Store;
using Music.WPF.ViewModels;
using System;

namespace Music.WPF.Services
{
    public sealed class NavigationService<TViewModel> : INavigationService where TViewModel : BaseViewModel
    {
        private readonly NavigationStore _navigationStore;
        private readonly IServiceProvider _serviceProvider;
        private readonly Func<TViewModel> _createViewModel;

        public NavigationService(NavigationStore navigationStore, Func<TViewModel> createViewModel)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
        }

        public NavigationService(NavigationStore navigationStore, IServiceProvider serviceProvider)
        {
            _navigationStore = navigationStore;
            _serviceProvider = serviceProvider;
        }

        public void Navigate()
        {
            _navigationStore.CurrentViewModel = _createViewModel();
        }

        public void Navigate<ViewModel>() where ViewModel : BaseViewModel
        {
            if (_serviceProvider is null) return;

            _navigationStore.CurrentViewModel = _serviceProvider.GetRequiredService<ViewModel>();
        }
    }
}

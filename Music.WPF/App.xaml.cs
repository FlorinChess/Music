using Microsoft.Extensions.DependencyInjection;
using Music.APIs;
using Music.APIs.Spotify;
using Music.NAudio.WaveformGenerator;
using Music.WPF.Commands;
using Music.WPF.Modals.ViewModels;
using Music.WPF.Models;
using Music.WPF.Services;
using Music.WPF.Store;
using Music.WPF.ViewModels;
using Music.WPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Music.WPF
{
    public sealed partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPersistenceService _persistenceService;
        public App()
        {
            IServiceCollection services = new ServiceCollection();
            
            ConfigureServices(services);
            
            _serviceProvider = services.BuildServiceProvider();

            _persistenceService = _serviceProvider.GetRequiredService<PersistenceService>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var trackStore = _serviceProvider.GetRequiredService<TrackStore>();
            trackStore.AddTracks(GetAvailableTracks().ToList());

            var initialNavigationService = _serviceProvider.GetRequiredService<INavigationService>();
            initialNavigationService.Navigate();

            MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow.Show();
            MainWindow.Closing += OnClosing; 

            _persistenceService.Parse();
            base.OnStartup(e);
        }

        private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
            mainViewModel.OnClosing();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Add Stores as Singletons
            services.AddSingleton<TrackStore>();
            services.AddSingleton<NavigationStore>();
            services.AddSingleton<ModalNavigationStore>();

            services.AddHttpClient();
            services.AddSingleton<SpotifyService>();
            services.AddTransient<MusicMetadataService>();
            services.AddSingleton<PersistenceService>(s => new (s.GetRequiredService<TrackStore>()));
            services.AddSingleton<WaveformGenerator>();

            // Add NavigationService as a Singleton
            services.AddSingleton<INavigationService>(s => CreateMyMusicNavigationService(s));
            // Add ViewModels 
            services.AddSingleton<MyMusicViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddSingleton<TrackListComponentViewModel>();
            services.AddTransient<PlaylistCollectionViewModel>();
            services.AddTransient<NowPlayingViewModel>();
            services.AddTransient<NowPlayingView>();
            services.AddTransient<SearchViewModel>();
            services.AddTransient<ListComponentViewModel>();
            services.AddTransient<LoadingComponentViewModel>();
            services.AddTransient<SelectedPlaylistViewModel>();

            // Modals
            services.AddTransient<AddToPlaylistModalViewModel>();
            services.AddTransient<EditPlaylistModalViewModel>();
            services.AddTransient<NewPlaylistModalViewModel>();

            // Modal commands
            services.AddSingleton<CloseModalCommand>();

            // Add NavigationBar as Singleton
            services.AddSingleton<NavigationBarViewModel>(s => CreateNavigationBarViewModel(s));

            // Add MusicPlayer as Singleton
            services.AddSingleton<MusicPlayerViewModel>();

            // Add MainViewModel and MainWindow as Singletons
            services.AddSingleton<MainViewModel>();
            // Add MainWindow as Singleton
            // Set it's DataContext to the MainViewModel
            services.AddSingleton<MainWindow>(s => new MainWindow()
            {
                DataContext = s.GetRequiredService<MainViewModel>(),
            });
        }

        private static INavigationService CreateMyMusicNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<MyMusicViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<MyMusicViewModel>());
        }

        private static INavigationService CreatePlaylistCollectionNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<PlaylistCollectionViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<PlaylistCollectionViewModel>());
        }

        private static INavigationService CreateSettingsNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<SettingsViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<SettingsViewModel>());
        }

        private static INavigationService CreateSearchNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<SearchViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<SearchViewModel>());
        }

        private static NavigationBarViewModel CreateNavigationBarViewModel(IServiceProvider serviceProvider)
        {
            return new NavigationBarViewModel(CreateMyMusicNavigationService(serviceProvider),
                CreatePlaylistCollectionNavigationService(serviceProvider),
                CreateSettingsNavigationService(serviceProvider),
                CreateSearchNavigationService(serviceProvider));
        }

        private static IEnumerable<TrackModel> GetAvailableTracks()
        {
            var musicFolders = MusicFilesService.GetMusicFolders();

            // Get paths of available music files
            var files = MusicFilesService.GetMusicFiles(musicFolders);

            // Create and return Track objects
            return TrackFactory.CreateTracks(files);
        }
    }
}

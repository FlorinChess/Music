using Microsoft.Extensions.DependencyInjection;
using Music.APIs;
using Music.APIs.Spotify;
using Music.Domain;
using Music.NAudio.WaveformGenerator;
using Music.WPF.Commands;
using Music.WPF.Modals.ViewModels;
using Music.WPF.Models;
using Music.WPF.Services;
using Music.WPF.Store;
using Music.WPF.ViewModels;
using Music.WPF.Views;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Music.WPF
{
    public sealed partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PlaylistPersistenceService _persistenceService;
        public App()
        {
            IServiceCollection services = new ServiceCollection();
            
            ConfigureServices(services);
            
            _serviceProvider = services.BuildServiceProvider();

            _persistenceService = _serviceProvider.GetRequiredService<PlaylistPersistenceService>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            var trackStore = _serviceProvider.GetRequiredService<TrackStore>();
            trackStore.AddTracks(GetAvailableTracks());
            trackStore.AddPlaylists(GetAvailablePlaylists(_serviceProvider));

            var initialNavigationService = _serviceProvider.GetRequiredService<INavigationService>();
            initialNavigationService.Navigate();

            MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow.Show();
            MainWindow.Closing += OnClosing;

            _persistenceService.Parse();

            CheckCommandLineParameters(Environment.GetCommandLineArgs()[1..], _serviceProvider);
            base.OnStartup(e);
        }

        private static void CheckCommandLineParameters(string[] args, IServiceProvider serviceProvider)
        {
            var trackStore = serviceProvider.GetRequiredService<TrackStore>();
            for (int i = 0; i < args.Length; i++)
            {
                if (!File.Exists(args[i])) continue;

                var track = TrackFactory.CreateTrack(args[i]);

                if (track is null) continue;

                trackStore.SetQueue(track);
            }
        }

        #region Private Methods

        private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
            mainViewModel.OnClosing();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<ILogger>(_ => new LoggerConfiguration().CreateLogger());

            // Add Stores as Singletons
            services.AddSingleton<TrackStore>();
            services.AddSingleton<NavigationStore>();
            services.AddSingleton<ModalNavigationStore>();

            services.AddSingleton<SpotifyService>();
            services.AddTransient<MusicMetadataService>();
            services.AddSingleton<PlaylistPersistenceService>();
            services.AddSingleton<WaveformGenerator>();

            // Add NavigationService as a Singleton
            services.AddSingleton<INavigationService>(s => CreateMyMusicNavigationService(s!));
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

        private static NavigationService<MyMusicViewModel> CreateMyMusicNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<MyMusicViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                serviceProvider.GetRequiredService<MyMusicViewModel>);
        }

        private static NavigationService<PlaylistCollectionViewModel> CreatePlaylistCollectionNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<PlaylistCollectionViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                serviceProvider.GetRequiredService<PlaylistCollectionViewModel>);
        }

        private static NavigationService<SettingsViewModel> CreateSettingsNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<SettingsViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                serviceProvider.GetRequiredService<SettingsViewModel>);
        }

        private static NavigationService<SearchViewModel> CreateSearchNavigationService(IServiceProvider serviceProvider)
        {
            return new NavigationService<SearchViewModel>(
                serviceProvider.GetRequiredService<NavigationStore>(),
                serviceProvider.GetRequiredService<SearchViewModel>);
        }

        private static NavigationBarViewModel CreateNavigationBarViewModel(IServiceProvider serviceProvider)
        {
            return new NavigationBarViewModel(CreateMyMusicNavigationService(serviceProvider),
                CreatePlaylistCollectionNavigationService(serviceProvider),
                CreateSettingsNavigationService(serviceProvider),
                CreateSearchNavigationService(serviceProvider),
                serviceProvider.GetRequiredService<NavigationStore>());
        }

        private static IList<TrackModel> GetAvailableTracks()
        {
            var musicFolders = MusicFilesService.GetMusicFolders();

            // Get paths of available music files
            var files = MusicFilesService.GetMusicFiles(musicFolders);

            // Create and return Track objects
            return TrackFactory.CreateTracks(files);
        }

        private static List<PlaylistModel> GetAvailablePlaylists(IServiceProvider serviceProvider)
        {
            var persistenceService = serviceProvider.GetRequiredService<PlaylistPersistenceService>();
            var root = persistenceService.Parse();
            var trackStore = serviceProvider.GetRequiredService<TrackStore>();

            var playlists = new List<PlaylistModel>();

            for (int i = 0; i < root?.Playlists.Count; i++)
            {
                var currentPlaylist = root.Playlists[i];

                playlists.Add(new PlaylistModel()
                {
                    Name = currentPlaylist.Name,
                    DateCreated = DateOnly.Parse(currentPlaylist.DateCreatedString),
                    ImagePath = currentPlaylist.ImagePath,
                });

                for (int j = 0; j < currentPlaylist.TracksFilePaths.Count; j++)
                {
                    try
                    {
                        playlists[i].Tracks.Add(trackStore.GetTrackByFilePath(currentPlaylist.TracksFilePaths[j]));
                    }
                    catch { }

                }
            }

            return playlists;
        }

        #endregion
    }
}

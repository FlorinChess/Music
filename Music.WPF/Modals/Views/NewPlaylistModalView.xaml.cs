using Music.WPF.Services;
using System.Windows.Controls;

namespace Music.WPF.Modals.Views
{
    public sealed partial class NewPlaylistModalView : UserControl
    {
        public NewPlaylistModalView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            playlistNameTextBox.Focus();
            playlistNameTextBox.SelectAll();

            var viewModel = (IRequestFocus)DataContext;
            viewModel.FocusRequested += OnFocusRequested;
        }

        private void OnFocusRequested(object? sender, FocusRequestedEventArgs e)
        {
            playlistNameTextBox.Focus();
            playlistNameTextBox.SelectAll();
        }
    }
}

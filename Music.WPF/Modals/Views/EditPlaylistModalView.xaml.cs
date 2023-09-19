using System.Windows.Controls;

namespace Music.WPF.Modals.Views
{
    public sealed partial class EditPlaylistModalView : UserControl
    {
        public EditPlaylistModalView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            playlistNameTextBox.Focus();
            playlistNameTextBox.SelectAll();
        }
    }
}

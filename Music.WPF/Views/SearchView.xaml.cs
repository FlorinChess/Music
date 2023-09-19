using System.Windows;
using System.Windows.Controls;

namespace Music.WPF.Views
{
    public sealed partial class SearchView : UserControl
    {
        public SearchView()
        {
            InitializeComponent();
        }

        private void OnSearchViewLoaded(object sender, RoutedEventArgs e)
        {
            searchTextBox.Focus();
            searchTextBox.SelectAll();
        }
    }
}

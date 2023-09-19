using System.Windows;
using System.Windows.Controls;

namespace Music.WPF.Components
{
    public sealed partial class MusicPlayer : UserControl
    {
        public bool HideSidePanels
        {
            get { return (bool)GetValue(HideSidePanelsProperty); }
            set { SetValue(HideSidePanelsProperty, value); }
        }

        public static readonly DependencyProperty HideSidePanelsProperty =
            DependencyProperty.Register("HideSidePanels", typeof(bool), typeof(MusicPlayer), 
                new PropertyMetadata(false, OnSidePanelVisibilityChanged));

        private static void OnSidePanelVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MusicPlayer musicPlayer)
            {
                musicPlayer.OnSidePanelVisibilityChanged();
            }
        }

        private void OnSidePanelVisibilityChanged()
        {
            if (HideSidePanels)
            {
                leftPanel.Visibility = Visibility.Collapsed;
                rightPanel.Visibility = Visibility.Collapsed;
            }
        }

        public MusicPlayer()
        {
            InitializeComponent();
        }
    }
}

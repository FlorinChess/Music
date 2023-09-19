using System.Windows;

namespace Music.WPF
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Window resizer
            WindowResizer windowResizer = new (this);

            MinimizeButton.Click += delegate { WindowState = WindowState.Minimized; };
            MaximizeButton.Click += delegate { WindowState ^= WindowState.Maximized; };
            CloseButton.Click += delegate { Application.Current.Shutdown(); };
        }
    }
}

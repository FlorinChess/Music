using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Music.CustomControls.Modal
{
    public sealed class Modal : ContentControl
    {
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(Modal), new PropertyMetadata(false));


        static Modal()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Modal), new FrameworkPropertyMetadata(typeof(Modal)));
            BackgroundProperty.OverrideMetadata(typeof(Modal), new FrameworkPropertyMetadata(CreateDefaultBackground()));
        }

        private static SolidColorBrush CreateDefaultBackground()
        {
            return new SolidColorBrush(Colors.Black)
            {
                Opacity = 0.5
            };
        }
    }
}

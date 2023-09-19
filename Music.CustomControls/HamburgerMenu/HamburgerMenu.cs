using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Music.CustomControls.HamburgerMenu
{
    public sealed class HamburgerMenu : Control
    {
        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }

        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(double), typeof(HamburgerMenu), new PropertyMetadata(50.0));

        public double FallbackWidth
        {
            get { return (double)GetValue(FallbackWidthProperty); }
            set { SetValue(FallbackWidthProperty, value); }
        }

        public static readonly DependencyProperty FallbackWidthProperty =
            DependencyProperty.Register("FallbackWidth", typeof(double), typeof(HamburgerMenu), new PropertyMetadata(100.0));

        public Duration OpenCloseDuration
        {
            get { return (Duration)GetValue(OpenCloseDurationProperty); }
            set { SetValue(OpenCloseDurationProperty, value); }
        }

        public static readonly DependencyProperty OpenCloseDurationProperty =
            DependencyProperty.Register("OpenCloseDuration", typeof(Duration), typeof(HamburgerMenu), new PropertyMetadata(Duration.Automatic));

        public FrameworkElement Content
        {
            get { return (FrameworkElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(FrameworkElement), typeof(HamburgerMenu), new PropertyMetadata(null));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(HamburgerMenu), new PropertyMetadata(false, OnIsOpenPropertyChanged));

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HamburgerMenu hamburgerMenu)
            {
                hamburgerMenu.OnIsOpenPropertyChanged();
            }
        }

        private void OnIsOpenPropertyChanged()
        {
            if (IsOpen)
            {
                OpenMenuAnimated();
            }
            else
            {
                CloseMenuAnimated();
            }
        }

        private void CloseMenuAnimated()
        {
            DoubleAnimation closingAnimation = new (IconWidth, OpenCloseDuration);
            BeginAnimation(WidthProperty, closingAnimation);
        }

        private void OpenMenuAnimated()
        {
            double width = GetDesiredContentWidth();

            DoubleAnimation openingAnimation = new (width, OpenCloseDuration);
            BeginAnimation(WidthProperty, openingAnimation);
        }

        private double GetDesiredContentWidth()
        {
            if (Content is null)
            {
                return FallbackWidth;
            }
            Content.Measure(new Size(MaxWidth, MaxHeight));
            double width = Content.DesiredSize.Width;
            return width;
        }

        static HamburgerMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HamburgerMenu), new FrameworkPropertyMetadata(typeof(HamburgerMenu)));
        }
        public HamburgerMenu()
        {
            Width = IconWidth;
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Music.WPF.Views
{
    public sealed partial class NowPlayingView : UserControl
    {
        private readonly PropertyPath _path = new (OpacityProperty);
        private readonly Duration _duration = new (TimeSpan.FromMilliseconds(600));

        public bool ShowWindow
        {
            get { return (bool)GetValue(ShowWindowProperty); }
            set { SetValue(ShowWindowProperty, value); }
        }

        public static readonly DependencyProperty ShowWindowProperty =
            DependencyProperty.Register("ShowWindow", typeof(bool), typeof(NowPlayingView), new PropertyMetadata(false, OnShowWindowChanged));

        private static void OnShowWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NowPlayingView userControl)
            {
                if ((bool)e.NewValue == true)
                    userControl.Show();
                else
                    userControl.Hide();
            }
        }

        public NowPlayingView()
        {
            Opacity = 0;
            InitializeComponent();
        }

        private void Show()
        {
            Keyboard.ClearFocus();

            Visibility = Visibility.Visible;

            DoubleAnimation animation = new()
            {
                From = 0.0,
                To = 1.0,
                FillBehavior = FillBehavior.HoldEnd,
                Duration = _duration
            };
            Storyboard storyboard = new();

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, _path);
            storyboard.Begin();
        }

        private void Hide()
        {
            var animation = new DoubleAnimation()
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                Duration = _duration
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, _path);
            storyboard.Completed += delegate 
            { 
                Visibility = Visibility.Collapsed; 
            };
            storyboard.Begin();
        }
    }
}

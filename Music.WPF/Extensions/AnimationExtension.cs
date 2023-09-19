using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Music.WPF.Extensions
{
    public static class AnimationExtension
    {
        public static void ToggleControlFade(this Control control)
        {
            var storyboard = new Storyboard();
            var duration = new TimeSpan(0, 0, 0, 0, 600); 

            var animation = new DoubleAnimation { From = 1.0, To = 0.0, Duration = new Duration(duration) };
            if (control.Opacity == 0.0)
            {
                animation = new DoubleAnimation { From = 0.0, To = 1.0, Duration = new Duration(duration) };
            }

            Storyboard.SetTarget(animation, control);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity", 0));
            storyboard.Children.Add(animation);

            storyboard.Begin(control);
        }

        public static void FadeOut(this Control control)
        {
            control.Visibility = Visibility.Visible;

            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromMilliseconds(2),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, control);
            Storyboard.SetTargetProperty(animation, new PropertyPath(control.Opacity));
            storyboard.Completed += delegate { control.Visibility = Visibility.Hidden; };
            storyboard.Begin();
        }
    }
}

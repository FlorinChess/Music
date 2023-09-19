using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Music.CustomControls.AnimatedListView
{
    [TemplatePart(Name = "PART_AnimatedVerticalScrollBar", Type = typeof(ScrollBar))]
    [TemplatePart(Name = "PART_AnimatedHorizontalScrollBar", Type = typeof(ScrollBar))]
    public sealed class AnimatedScrollViewer : ScrollViewer
    {
        #region Private Members

        private ScrollBar _animatedVerticalScrollBar;
        private ScrollBar _animatedHorizontalScrollBar;
        private readonly DoubleAnimationUsingKeyFrames _animation = new();
        private readonly SplineDoubleKeyFrame _keyFrame = new();

        #endregion Private Members

        static AnimatedScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedScrollViewer), new FrameworkPropertyMetadata(typeof(AnimatedScrollViewer)));
        }

        #region ScrollViewer Override Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            if (GetTemplateChild("PART_AnimatedVerticalScrollBar") is ScrollBar animatedVerticalScrollBar)
            {
                _animatedVerticalScrollBar = animatedVerticalScrollBar;
            }
            _animatedVerticalScrollBar.ValueChanged += OnVerticalScrollBarValueChanged;

            if (GetTemplateChild("PART_AnimatedHorizontalScrollBar") is ScrollBar animatedHorizontalScrollBar)
            {
                _animatedHorizontalScrollBar = animatedHorizontalScrollBar;
            }
            _animatedHorizontalScrollBar.ValueChanged += OnHorizontalScrollBarValueChanged;

            PreviewMouseWheel += OnPreviewMouseWheel;
        }

        #endregion

        #region Custom Event Handlers

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double mouseWheelChange = e.Delta;

            if (sender is AnimatedScrollViewer scrollViewer)
            {
                double newVerticalOffset = scrollViewer.TargetVerticalOffset - mouseWheelChange;
                if (newVerticalOffset < 0)
                {
                    scrollViewer.TargetVerticalOffset = 0;
                }
                else if (newVerticalOffset > scrollViewer.ScrollableHeight)
                {
                    scrollViewer.TargetVerticalOffset = scrollViewer.ScrollableHeight;
                }
                else
                {
                    scrollViewer.TargetVerticalOffset = newVerticalOffset;
                }
            }

            e.Handled = true;
        }

        private void OnVerticalScrollBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double oldTargetVerticalOffset = (double)e.OldValue;
            double newTargetVerticalOffset = (double)e.NewValue;

            if (newTargetVerticalOffset != TargetVerticalOffset)
            {
                double deltaVerticalOffset = Math.Round(newTargetVerticalOffset - oldTargetVerticalOffset, 3);

                switch (deltaVerticalOffset)
                {
                    case 1:
                        TargetVerticalOffset = oldTargetVerticalOffset + ViewportHeight;
                        break;
                    case -1:
                        TargetVerticalOffset = oldTargetVerticalOffset - ViewportHeight;
                        break;
                    case 0.1:
                        TargetVerticalOffset = oldTargetVerticalOffset + 16.0;
                        break;
                    case -0.1:
                        TargetVerticalOffset = oldTargetVerticalOffset - 16.0;
                        break;
                    default:
                        TargetVerticalOffset = newTargetVerticalOffset;
                        break;
                }
            }
        }

        private void OnHorizontalScrollBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double oldTargetHOffset = (double)e.OldValue;
            double newTargetHOffset = (double)e.NewValue;

            if (newTargetHOffset != TargetHorizontalOffset)
            {

                double deltaVOffset = Math.Round(newTargetHOffset - oldTargetHOffset, 3);

                switch (deltaVOffset)
                {
                    case 1:
                        TargetHorizontalOffset = oldTargetHOffset + ViewportWidth;
                        break;
                    case -1:
                        TargetHorizontalOffset = oldTargetHOffset - ViewportWidth;
                        break;
                    case 0.1:
                        TargetHorizontalOffset = oldTargetHOffset + 16.0;
                        break;
                    case -0.1:
                        TargetHorizontalOffset = oldTargetHOffset - 16.0;
                        break;
                    default:
                        TargetHorizontalOffset = newTargetHOffset;
                        break;
                }
            }
        }

        #endregion

        #region Custom Dependency Properties

        #region TargetVerticalOffset (DependencyProperty)(double)

        /// <summary>
        /// This is the VerticalOffset that we'd like to animate
        /// </summary>
        public double TargetVerticalOffset
        {
            get { return (double)GetValue(TargetVerticalOffsetProperty); }
            set { SetValue(TargetVerticalOffsetProperty, value); }
        }
        public static readonly DependencyProperty TargetVerticalOffsetProperty =
            DependencyProperty.Register("TargetVerticalOffset", typeof(double), typeof(AnimatedScrollViewer),
            new PropertyMetadata(0.0, OnTargetVerticalOffsetChanged));

        private static void OnTargetVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AnimatedScrollViewer animatedScrollViewer)
            {
                if ((double)e.NewValue != animatedScrollViewer._animatedVerticalScrollBar.Value)
                {
                    animatedScrollViewer._animatedVerticalScrollBar.Value = (double)e.NewValue;
                }

                animatedScrollViewer.AnimateScroller(Orientation.Vertical);
            }
        }

        #endregion

        #region TargetHorizontalOffset (DependencyProperty) (double)

        /// <summary>
        /// This is the HorizontalOffset that we'll be animating
        /// </summary>
        public double TargetHorizontalOffset
        {
            get => (double)GetValue(TargetHorizontalOffsetProperty);
            set => SetValue(TargetHorizontalOffsetProperty, value); 
        }
        public static readonly DependencyProperty TargetHorizontalOffsetProperty =
            DependencyProperty.Register("TargetHorizontalOffset", typeof(double), typeof(AnimatedScrollViewer),
            new PropertyMetadata(0.0, OnTargetHorizontalOffsetChanged));

        private static void OnTargetHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is  AnimatedScrollViewer animatedScrollViewer)
            {
                if ((double)e.NewValue != animatedScrollViewer._animatedHorizontalScrollBar.Value)
                {
                    animatedScrollViewer._animatedHorizontalScrollBar.Value = (double)e.NewValue;
                }
                animatedScrollViewer.AnimateScroller(Orientation.Horizontal);
            }
        }

        #endregion

        #region HorizontalScrollOffset (DependencyProperty) (double)

        /// <summary>
        /// This is the actual horizontal offset property we're going use as an animation helper
        /// </summary>
        public double HorizontalScrollOffset
        {
            get => (double)GetValue(HorizontalScrollOffsetProperty); 
            set => SetValue(HorizontalScrollOffsetProperty, value);
        }
        public static readonly DependencyProperty HorizontalScrollOffsetProperty =
            DependencyProperty.Register("HorizontalScrollOffset", typeof(double), typeof(AnimatedScrollViewer),
            new PropertyMetadata(0.0, OnHorizontalScrollOffsetChanged));

        private static void OnHorizontalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatedScrollViewer thisSViewer = (AnimatedScrollViewer)d;
            thisSViewer.ScrollToHorizontalOffset((double)e.NewValue);
        }

        #endregion

        #region VerticalScrollOffset (DependencyProperty) (double)

        /// <summary>
        /// This is the actual VerticalOffset we're going to use as an animation helper
        /// </summary>
        public double VerticalScrollOffset
        {
            get => (double)GetValue(VerticalScrollOffsetProperty);
            set => SetValue(VerticalScrollOffsetProperty, value);
        }
        public static readonly DependencyProperty VerticalScrollOffsetProperty =
            DependencyProperty.Register("VerticalScrollOffset", typeof(double), typeof(AnimatedScrollViewer),
            new PropertyMetadata(0.0, OnVerticalScrollOffsetChanged));

        private static void OnVerticalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatedScrollViewer scrollViewer = (AnimatedScrollViewer)d;
            scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        #endregion

        #region AnimationTime (DependencyProperty) (TimeSpan)

        /// <summary>
        /// A property for changing the time it takes to scroll to a new position. 
        /// </summary>
        public TimeSpan ScrollingTime
        {
            get => (TimeSpan)GetValue(ScrollingTimeProperty);
            set => SetValue(ScrollingTimeProperty, value);
        }
        public static readonly DependencyProperty ScrollingTimeProperty =
            DependencyProperty.Register("ScrollingTime", typeof(TimeSpan), typeof(AnimatedScrollViewer),
              new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 300)));

        #endregion

        #region ScrollingSpline (DependencyProperty)

        /// <summary>
        /// A property to allow users to describe a custom spline for the scrolling animation.
        /// </summary>
        public KeySpline ScrollingSpline
        {
            get => (KeySpline)GetValue(ScrollingSplineProperty);
            set => SetValue(ScrollingSplineProperty, value);
        }
        public static readonly DependencyProperty ScrollingSplineProperty =
            DependencyProperty.Register("ScrollingSpline", typeof(KeySpline), typeof(AnimatedScrollViewer),
              new PropertyMetadata(new KeySpline(0.000, 0.05, 0.05, 1)));

        #endregion ScrollingSpline (DependencyProperty)

        #endregion

        #region AnimateScroller method (Creates and runs animation)
        public void AnimateScroller(Orientation orientation)
        {
            _animation.KeyFrames.Clear();
            
            _keyFrame.KeyTime = ScrollingTime;
            _keyFrame.KeySpline = ScrollingSpline;

            switch (orientation)
            {
                case Orientation.Horizontal:
                    _keyFrame.Value = TargetHorizontalOffset;

                    // add KeyFrame
                    _animation.KeyFrames.Add(_keyFrame);

                    // begin Animation
                    BeginAnimation(HorizontalScrollOffsetProperty, _animation);
                    break;
                case Orientation.Vertical:
                    _keyFrame.Value = TargetVerticalOffset;

                    // add KeyFrame
                    _animation.KeyFrames.Add(_keyFrame);

                    // begin Animation
                    BeginAnimation(VerticalScrollOffsetProperty, _animation);
                    break;
            }
        }
        #endregion
    }
}

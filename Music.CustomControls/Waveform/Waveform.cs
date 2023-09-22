using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Music.CustomControls.Waveform
{
    [TemplatePart(Name = "PART_Waveform", Type = typeof(Canvas))]
    public sealed class Waveform : Control
    {
        private Canvas _waveformCanvas;
        private readonly Line _centerLine = new() { StrokeThickness = 1 };

        public float[] WaveformData
        {
            get { return (float[])GetValue(WaveformDataProperty); }
            set
            {
                SetValue(WaveformDataProperty, value);
                UpdateWaveform();
            }
        }

        public static readonly DependencyProperty WaveformDataProperty =
            DependencyProperty.Register("WaveformData", typeof(float[]), typeof(Waveform),
                new PropertyMetadata(new float[1], OnWaveformDataChanged));

        private static void OnWaveformDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Waveform timeline)
            {
                timeline.OnWaveformDataChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnWaveformDataChanged(object oldValue, object newValue)
        {
            if (oldValue == newValue) return;

            WaveformData = (float[])GetValue(WaveformDataProperty);
        }

        public Brush CenterLineBrush
        {
            get { return (Brush)GetValue(CenterLineBrushProperty); }
            set { SetValue(CenterLineBrushProperty, value); }
        }

        public static readonly DependencyProperty CenterLineBrushProperty =
            DependencyProperty.Register("CenterLineBrush", typeof(Brush), typeof(Waveform),
                new PropertyMetadata(new SolidColorBrush(Colors.Black), OnCenterLineBrushChanged, OnCoerceCenterLineBrush));

        private static object OnCoerceCenterLineBrush(DependencyObject d, object value)
        {
            if (d is Waveform waveformTimeline)
                return waveformTimeline.OnCoerceCenterLineBrush((Brush)value);
            else
                return value;
        }

        private object OnCoerceCenterLineBrush(Brush value)
        {
            return value;
        }

        private static void OnCenterLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Waveform timeline)
            {
                timeline.OnCenterLineBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
            }
        }

        private void OnCenterLineBrushChanged(Brush oldValue, Brush newValue)
        {
            if (oldValue == newValue) return;

            _centerLine.Fill = RightLevelBrush;
            UpdateWaveform();
        }

        public Brush LeftLevelBrush
        {
            get { return (Brush)GetValue(LeftLevelBrushProperty); }
            set { SetValue(LeftLevelBrushProperty, value); }
        }

        public static readonly DependencyProperty LeftLevelBrushProperty =
            DependencyProperty.Register("LeftLevelBrush", typeof(Brush), typeof(Waveform),
                new PropertyMetadata(new SolidColorBrush(Colors.Green), OnLeftLevelBrushChanged, OnCoerceLeftLevelBrush));

        private static object OnCoerceLeftLevelBrush(DependencyObject d, object value)
        {
            if (d is Waveform waveformTimeline)
                return waveformTimeline.OnCoerceLeftLevelBrush((Brush)value);
            else
                return value;
        }

        private object OnCoerceLeftLevelBrush(Brush value)
        {
            return value;
        }

        private static void OnLeftLevelBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Waveform timeline)
            {
                timeline.OnLeftLevelBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
            }
        }

        private void OnLeftLevelBrushChanged(Brush oldValue, Brush newValue)
        {
            if (oldValue == newValue) return;

            UpdateWaveform();
        }

        public Brush RightLevelBrush
        {
            get { return (Brush)GetValue(RightLevelBrushProperty); }
            set { SetValue(RightLevelBrushProperty, value); }
        }

        public static readonly DependencyProperty RightLevelBrushProperty =
            DependencyProperty.Register("RightLevelBrush", typeof(Brush), typeof(Waveform),
                new PropertyMetadata(new SolidColorBrush(Colors.LightBlue), OnRightLevelBrushChanged, OnCoerceRightLevelBrush));

        private static object OnCoerceRightLevelBrush(DependencyObject d, object value)
        {
            if (d is Waveform waveformTimeline)
                return waveformTimeline.OnCoerceRightLevelBrush((Brush)value);
            else
                return value;
        }

        private object OnCoerceRightLevelBrush(Brush baseValue)
        {
            return baseValue;
        }

        private static void OnRightLevelBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Waveform timeline)
            {
                timeline.OnRightLevelBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
            }
        }

        private void OnRightLevelBrushChanged(Brush oldValue, Brush newValue)
        {
            if (oldValue == newValue) return;

            UpdateWaveform();
        }

        static Waveform()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Waveform), new FrameworkPropertyMetadata(typeof(Waveform)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _waveformCanvas = (Canvas)GetTemplateChild("PART_Waveform");
            _waveformCanvas.CacheMode = new BitmapCache();
            _waveformCanvas.Background = new SolidColorBrush(Colors.Transparent);

            UpdateWaveformCacheScaling();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            UpdateWaveformCacheScaling();
            UpdateWaveform();
        }

        // Simplified
        private void UpdateWaveformCacheScaling()
        {
            if (_waveformCanvas is null) return;

            BitmapCache waveformCache = (BitmapCache)_waveformCanvas.CacheMode;

            waveformCache.RenderAtScale = 1.0d;
        }

        private void UpdateWaveform()
        {
            if (WaveformData is null || WaveformData.Length < 100 || _waveformCanvas is null ||
                _waveformCanvas.RenderSize.Width < 1 || _waveformCanvas.RenderSize.Height < 1)
                return;

            _waveformCanvas?.Children.Clear();

            const double minimumValue = 0;
            const double maximumValue = 1.5;
            const double dbScale = (maximumValue - minimumValue);

            double leftRenderHeight;
            double rightRenderHeight;

            int multiplier = 3;
            
            // WaveformData contains data for upper and lower, so the pointCount per section is WaveformData.Length / 2
            // WaveformData default 1500
            // 750 samples per channel, 750 / 3 = 250 rectangles
            int rectangleCount = 250;


            // Width and Margin calculation: 
            // 1. Space per Rectangle: _waveformCanvas.RenderSize.Width / rectangleCount
            double spacePerRectangle = _waveformCanvas!.RenderSize.Width / rectangleCount;

            // 2. width: 80% of spacePerRectangle rounded to whole number
            double rectangleWidth = Math.Round(0.8d * spacePerRectangle);

            // 3. Margin: spacePerRectangle - width
            double marginTotal = Math.Round(spacePerRectangle - rectangleWidth, 2);
            double margin = marginTotal / 2.0d;

            double waveformMaxHeight = _waveformCanvas.RenderSize.Height / 2.0d; // Height of the canvas devided by two: one for upper, one for lower
            double centerHeight = waveformMaxHeight;

            List<double> rectTopHeights = new();
            List<double> rectBottomHeights = new();

            int countZero = 0;

            int rectCount = 0;
            for (int i = 0; i < WaveformData.Length; i += 2)
            {
                leftRenderHeight = ((WaveformData[i] - minimumValue) / dbScale) * waveformMaxHeight;
                rectTopHeights.Add(leftRenderHeight);

                rightRenderHeight = ((WaveformData[i + 1] - minimumValue) / dbScale) * waveformMaxHeight;
                rectBottomHeights.Add(rightRenderHeight);

                if (leftRenderHeight < 0.0)
                {
                    countZero++;
                }
                else
                {
                    countZero = 0; // Reset
                }

                if (countZero >= 3) // After 3 consecutive empty samples break;
                {
                    Debug.WriteLine($"i = { i }");
                }

                // Because we cut the number of samples, every 'multiplier' times (default = 3) add a new Rectangle
                if (rectTopHeights.Count % multiplier == 0)
                {
                    var average = rectTopHeights.Average();
                    average = Math.Round(average, 4);

                    // Don't add invisible rectagles
                    if (average <= 0.0004d) continue;

                    var marginObject = new Thickness(margin, 0, margin, 0);

                    Rectangle newRectTop = new()
                    {
                        Height = average,
                        Width = rectangleWidth,
                        Margin = marginObject,
                        Fill = LeftLevelBrush
                    };

                    Rectangle newRectBottom = new()
                    {
                        Height = average,
                        Width = rectangleWidth,
                        Margin = marginObject,
                        Fill = RightLevelBrush
                    };

                    double xPosition = rectCount * (rectangleWidth + margin);

                    Canvas.SetLeft(newRectTop, xPosition);
                    Canvas.SetBottom(newRectTop, centerHeight);

                    Canvas.SetLeft(newRectBottom, xPosition);
                    Canvas.SetTop(newRectBottom, centerHeight);

                    _waveformCanvas.Children.Add(newRectTop);
                    _waveformCanvas.Children.Add(newRectBottom);

                    rectTopHeights.Clear();
                    rectBottomHeights.Clear();

                    rectCount++;
                }
            }
        }
    }
}

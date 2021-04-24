﻿// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    public abstract class Chart : Control, IChartView<SkiaSharpDrawingContext>
    {
        #region fields

        protected Chart<SkiaSharpDrawingContext>? core;
        protected MotionCanvas? canvas;
        protected IChartLegend<SkiaSharpDrawingContext>? legend;
        protected IChartTooltip<SkiaSharpDrawingContext>? tooltip;
        private readonly ActionThrottler mouseMoveThrottler;
        private PointF mousePosition = new();

        #endregion

        public Chart()
        {
            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ConstructChart(this);

            SizeChanged += OnSizeChanged;
            MouseMove += OnMouseMove;
            mouseMoveThrottler = new ActionThrottler(MouseMoveThrottlerUnlocked, TimeSpan.FromMilliseconds(10));
        }

        #region dependendency properties

        public static readonly DependencyProperty DrawMarginProperty =
           DependencyProperty.Register(
               nameof(DrawMargin), typeof(Margin), typeof(Chart), new PropertyMetadata(null, OnDependencyPropertyChanged));

        public static readonly DependencyProperty AnimationsSpeedProperty =
            DependencyProperty.Register(
                nameof(AnimationsSpeed), typeof(TimeSpan), typeof(Chart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultAnimationsSpeed, OnDependencyPropertyChanged));

        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register(
                nameof(EasingFunction), typeof(Func<float, float>), typeof(Chart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultEasingFunction, OnDependencyPropertyChanged));

        public static readonly DependencyProperty LegendPositionProperty =
            DependencyProperty.Register(
                nameof(LegendPosition), typeof(LegendPosition), typeof(Chart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultLegendPosition, OnDependencyPropertyChanged));

        public static readonly DependencyProperty LegendOrientationProperty =
            DependencyProperty.Register(
                nameof(LegendOrientation), typeof(LegendOrientation), typeof(Chart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultLegendOrientation, OnDependencyPropertyChanged));

        public static readonly DependencyProperty TooltipPositionProperty =
           DependencyProperty.Register(
               nameof(TooltipPosition), typeof(TooltipPosition), typeof(Chart),
               new PropertyMetadata(LiveCharts.CurrentSettings.DefaultTooltipPosition, OnDependencyPropertyChanged));

        public static readonly DependencyProperty TooltipFindingStrategyProperty =
            DependencyProperty.Register(
                nameof(TooltipFindingStrategy), typeof(TooltipFindingStrategy), typeof(Chart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy, OnDependencyPropertyChanged));

        public static readonly DependencyProperty TooltipBackgroundProperty =
           DependencyProperty.Register(
               nameof(TooltipBackground), typeof(SolidColorBrush), typeof(Chart),
               new PropertyMetadata(new SolidColorBrush(System.Windows.Media.Color.FromRgb(250, 250, 250)), OnDependencyPropertyChanged));

        public static readonly DependencyProperty TooltipFontFamilyProperty =
           DependencyProperty.Register(
               nameof(TooltipFontFamily), typeof(FontFamily), typeof(Chart),
               new PropertyMetadata(new FontFamily("Trebuchet MS"), OnDependencyPropertyChanged));

        public static readonly DependencyProperty TooltipTextColorProperty =
           DependencyProperty.Register(
               nameof(TooltipTextColor), typeof(SolidColorBrush), typeof(Chart),
               new PropertyMetadata(new SolidColorBrush(System.Windows.Media.Color.FromRgb(35, 35, 35)), OnDependencyPropertyChanged));

        public static readonly DependencyProperty TooltipFontSizeProperty =
           DependencyProperty.Register(
               nameof(TooltipFontSize), typeof(double), typeof(Chart), new PropertyMetadata(13d, OnDependencyPropertyChanged));

        public static readonly DependencyProperty TooltipFontWeightProperty =
           DependencyProperty.Register(
               nameof(TooltipFontWeight), typeof(FontWeight), typeof(Chart),
               new PropertyMetadata(FontWeights.Normal, OnDependencyPropertyChanged));

        public static readonly DependencyProperty TooltipFontStretchProperty =
           DependencyProperty.Register(
               nameof(TooltipFontStretch), typeof(FontStretch), typeof(Chart),
               new PropertyMetadata(FontStretches.Normal, OnDependencyPropertyChanged));

        public static readonly DependencyProperty TooltipFontStyleProperty =
           DependencyProperty.Register(
               nameof(TooltipFontStyle), typeof(FontStyle), typeof(Chart),
               new PropertyMetadata(FontStyles.Normal, OnDependencyPropertyChanged));

        public static readonly DependencyProperty TooltipTemplateProperty =
            DependencyProperty.Register(
                nameof(TooltipTemplate), typeof(DataTemplate), typeof(Chart), new PropertyMetadata(null, OnDependencyPropertyChanged));

        public static readonly DependencyProperty LegendFontFamilyProperty =
           DependencyProperty.Register(
               nameof(LegendFontFamily), typeof(FontFamily), typeof(Chart),
               new PropertyMetadata(new FontFamily("Trebuchet MS"), OnDependencyPropertyChanged));

        public static readonly DependencyProperty LegendTextColorProperty =
           DependencyProperty.Register(
               nameof(LegendTextColor), typeof(SolidColorBrush), typeof(Chart),
               new PropertyMetadata(new SolidColorBrush(System.Windows.Media.Color.FromRgb(35, 35, 35)), OnDependencyPropertyChanged));

        public static readonly DependencyProperty LegendFontSizeProperty =
           DependencyProperty.Register(
               nameof(LegendFontSize), typeof(double), typeof(Chart), new PropertyMetadata(13d, OnDependencyPropertyChanged));

        public static readonly DependencyProperty LegendFontWeightProperty =
           DependencyProperty.Register(
               nameof(LegendFontWeight), typeof(FontWeight), typeof(Chart),
               new PropertyMetadata(FontWeights.Normal, OnDependencyPropertyChanged));

        public static readonly DependencyProperty LegendFontStretchProperty =
           DependencyProperty.Register(
               nameof(LegendFontStretch), typeof(FontStretch), typeof(Chart),
               new PropertyMetadata(FontStretches.Normal, OnDependencyPropertyChanged));

        public static readonly DependencyProperty LegendFontStyleProperty =
           DependencyProperty.Register(
               nameof(LegendFontStyle), typeof(FontStyle), typeof(Chart),
               new PropertyMetadata(FontStyles.Normal, OnDependencyPropertyChanged));

        public static readonly DependencyProperty LegendTemplateProperty =
            DependencyProperty.Register(
                nameof(LegendTemplate), typeof(DataTemplate), typeof(Chart), new PropertyMetadata(null, OnDependencyPropertyChanged));

        #endregion

        #region properties

        public Margin DrawMargin
        {
            get { return (Margin)GetValue(DrawMarginProperty); }
            set { SetValue(DrawMarginProperty, value); }
        }

        SizeF IChartView.ControlSize
        {
            get
            {
                if (canvas == null) throw new Exception("Canvas not found");
                return new() { Width = (float)canvas.ActualWidth, Height = (float)canvas.ActualHeight };
            }
        }

        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas
        {
            get
            {
                if (canvas == null) throw new Exception("Canvas not found");
                return canvas.CanvasCore;
            }
        }

        public TimeSpan AnimationsSpeed
        {
            get { return (TimeSpan)GetValue(AnimationsSpeedProperty); }
            set { SetValue(AnimationsSpeedProperty, value); }
        }

        public Func<float, float> EasingFunction
        {
            get { return (Func<float, float>)GetValue(EasingFunctionProperty); }
            set { SetValue(AnimationsSpeedProperty, value); }
        }

        public LegendPosition LegendPosition
        {
            get { return (LegendPosition)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        public LegendOrientation LegendOrientation
        {
            get { return (LegendOrientation)GetValue(LegendOrientationProperty); }
            set { SetValue(LegendOrientationProperty, value); }
        }

        public TooltipPosition TooltipPosition
        {
            get { return (TooltipPosition)GetValue(TooltipPositionProperty); }
            set { SetValue(TooltipPositionProperty, value); }
        }

        public TooltipFindingStrategy TooltipFindingStrategy
        {
            get { return (TooltipFindingStrategy)GetValue(TooltipFindingStrategyProperty); }
            set { SetValue(TooltipFindingStrategyProperty, value); }
        }

        public DataTemplate? TooltipTemplate
        {
            get { return (DataTemplate?)GetValue(TooltipTemplateProperty); }
            set { SetValue(TooltipTemplateProperty, value); }
        }

        public Brush TooltipBackground
        {
            get { return (Brush)GetValue(TooltipBackgroundProperty); }
            set { SetValue(TooltipBackgroundProperty, value); }
        }

        public FontFamily TooltipFontFamily
        {
            get { return (FontFamily)GetValue(TooltipFontFamilyProperty); }
            set { SetValue(TooltipFontFamilyProperty, value); }
        }

        public SolidColorBrush TooltipTextColor
        {
            get { return (SolidColorBrush)GetValue(TooltipTextColorProperty); }
            set { SetValue(TooltipTextColorProperty, value); }
        }

        public double TooltipFontSize
        {
            get { return (double)GetValue(TooltipFontSizeProperty); }
            set { SetValue(TooltipFontSizeProperty, value); }
        }

        public FontWeight TooltipFontWeight
        {
            get { return (FontWeight)GetValue(TooltipFontWeightProperty); }
            set { SetValue(TooltipFontWeightProperty, value); }
        }

        public FontStretch TooltipFontStretch
        {
            get { return (FontStretch)GetValue(TooltipFontStretchProperty); }
            set { SetValue(TooltipFontStretchProperty, value); }
        }

        public FontStyle TooltipFontStyle
        {
            get { return (FontStyle)GetValue(TooltipFontStyleProperty); }
            set { SetValue(TooltipFontStyleProperty, value); }
        }

        public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => tooltip;

        public DataTemplate? LegendTemplate
        {
            get { return (DataTemplate?)GetValue(LegendTemplateProperty); }
            set { SetValue(LegendTemplateProperty, value); }
        }

        public FontFamily LegendFontFamily
        {
            get { return (FontFamily)GetValue(LegendFontFamilyProperty); }
            set { SetValue(LegendFontFamilyProperty, value); }
        }

        public SolidColorBrush LegendTextColor
        {
            get { return (SolidColorBrush)GetValue(LegendTextColorProperty); }
            set { SetValue(LegendTextColorProperty, value); }
        }

        public double LegendFontSize
        {
            get { return (double)GetValue(LegendFontSizeProperty); }
            set { SetValue(LegendFontSizeProperty, value); }
        }

        public FontWeight LegendFontWeight
        {
            get { return (FontWeight)GetValue(LegendFontWeightProperty); }
            set { SetValue(LegendFontWeightProperty, value); }
        }

        public FontStretch LegendFontStretch
        {
            get { return (FontStretch)GetValue(LegendFontStretchProperty); }
            set { SetValue(LegendFontStretchProperty, value); }
        }

        public FontStyle LegendFontStyle
        {
            get { return (FontStyle)GetValue(LegendFontStyleProperty); }
            set { SetValue(LegendFontStyleProperty, value); }
        }

        public IChartLegend<SkiaSharpDrawingContext>? Legend => legend;

        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; } = new();

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template.FindName("canvas", this) is not MotionCanvas canvas)
                throw new Exception(
                    $"{nameof(MotionCanvas)} not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
                    $"If you override the template please add an {nameof(MotionCanvas)} to the template and name it 'canvas'");

            this.canvas = canvas;
            InitializeCore();
        }

        protected abstract void InitializeCore();

        protected static void OnDependencyPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            var chart = (CartesianChart)o;
            if (chart.core == null) return;
            Application.Current.Dispatcher.Invoke(() => chart.core.Update());
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (core == null) return;
            Application.Current.Dispatcher.Invoke(() => core.Update());
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var p = e.GetPosition(canvas);
            mousePosition = new PointF((float)p.X, (float)p.Y);
            mouseMoveThrottler.Call();
        }

        private void MouseMoveThrottlerUnlocked()
        {
            if (core == null || tooltip == null || TooltipPosition == TooltipPosition.Hidden) return;
            tooltip.Show(core.FindPointsNearTo(mousePosition), core);
        }
    }
}

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

using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using Xamarin.Forms;
using c = Xamarin.Forms.Color;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CartesianChart : ContentView, ICartesianChartView<SkiaSharpDrawingContext>, IMobileChart
    {
        #region fields

        private readonly CollectionDeepObserver<ISeries> seriesObserver;
        private readonly CollectionDeepObserver<IAxis> xObserver;
        private readonly CollectionDeepObserver<IAxis> yObserver;
        private Grid? grid;
        protected Chart<SkiaSharpDrawingContext>? core;

        #endregion

        public CartesianChart()
        {
            InitializeComponent();

            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ConstructChart(this);

            InitializeCore();
            SizeChanged += OnSizeChanged;

            seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            xObserver = new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            yObserver = new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

            XAxes = new List<IAxis>() { new Axis() };
            YAxes = new List<IAxis>() { new Axis() };
            Series = new ObservableCollection<ISeries>();

            canvas.SkCanvasView.EnableTouchEvents = true;
            canvas.SkCanvasView.Touch += OnSkCanvasTouched;
        }

        #region bindable properties 

        public static readonly BindableProperty SeriesProperty =
            BindableProperty.Create(
                nameof(Series), typeof(IEnumerable<ISeries>), typeof(CartesianChart), new ObservableCollection<ISeries>(), BindingMode.Default, null,
                (BindableObject o, object oldValue, object newValue) =>
                {
                    var chart = (CartesianChart)o;
                    var seriesObserver = chart.seriesObserver;
                    seriesObserver.Dispose((IEnumerable<ISeries>)oldValue);
                    seriesObserver.Initialize((IEnumerable<ISeries>)newValue);
                    if (chart.core == null) return;
                    MainThread.BeginInvokeOnMainThread(() => chart.core.Update());
                });

        public static readonly BindableProperty XAxesProperty =
            BindableProperty.Create(
                nameof(XAxes), typeof(IEnumerable<IAxis>), typeof(CartesianChart), new List<IAxis>() { new Axis() }, BindingMode.Default, null,
                (BindableObject o, object oldValue, object newValue) =>
                {
                    var chart = (CartesianChart)o;
                    var observer = chart.xObserver;
                    observer.Dispose((IEnumerable<IAxis>)oldValue);
                    observer.Initialize((IEnumerable<IAxis>)newValue);
                    if (chart.core == null) return;
                    MainThread.BeginInvokeOnMainThread(() => chart.core.Update());
                });

        public static readonly BindableProperty YAxesProperty =
            BindableProperty.Create(
                nameof(YAxes), typeof(IEnumerable<IAxis>), typeof(CartesianChart), new List<IAxis>() { new Axis() }, BindingMode.Default, null,
                (BindableObject o, object oldValue, object newValue) =>
                {
                    var chart = (CartesianChart)o;
                    var observer = chart.yObserver;
                    observer.Dispose((IEnumerable<IAxis>)oldValue);
                    observer.Initialize((IEnumerable<IAxis>)newValue);
                    if (chart.core == null) return;
                    MainThread.BeginInvokeOnMainThread(() => chart.core.Update());
                });

        public static readonly BindableProperty DrawMarginProperty =
            BindableProperty.Create(
                nameof(DrawMargin), typeof(Margin), typeof(CartesianChart), null, BindingMode.Default, null, OnBindablePropertyChanged);

        public static readonly BindableProperty ZoomModeProperty =
            BindableProperty.Create(
                nameof(ZoomMode), typeof(ZoomAndPanMode), typeof(CartesianChart),
                LiveCharts.CurrentSettings.DefaultZoomMode, BindingMode.Default, null);

        public static readonly BindableProperty ZoomingSpeedProperty =
            BindableProperty.Create(
                nameof(ZoomingSpeed), typeof(double), typeof(CartesianChart),
                LiveCharts.CurrentSettings.DefaultZoomSpeed, BindingMode.Default, null);

        public static readonly BindableProperty AnimationsSpeedProperty =
           BindableProperty.Create(
               nameof(AnimationsSpeed), typeof(TimeSpan), typeof(CartesianChart), LiveCharts.CurrentSettings.DefaultAnimationsSpeed);

        public static readonly BindableProperty EasingFunctionProperty =
            BindableProperty.Create(
                nameof(EasingFunction), typeof(Func<float, float>), typeof(CartesianChart),
                LiveCharts.CurrentSettings.DefaultEasingFunction);

        public static readonly BindableProperty LegendPositionProperty =
            BindableProperty.Create(
                nameof(LegendPosition), typeof(LegendPosition), typeof(CartesianChart),
                LiveCharts.CurrentSettings.DefaultLegendPosition, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty LegendOrientationProperty =
            BindableProperty.Create(
                nameof(LegendOrientation), typeof(LegendOrientation), typeof(CartesianChart),
                LiveCharts.CurrentSettings.DefaultLegendOrientation, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty LegendTemplateProperty =
            BindableProperty.Create(
                nameof(LegendTemplate), typeof(DataTemplate), typeof(CartesianChart), null, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty LegendFontFamilyProperty =
            BindableProperty.Create(
                nameof(LegendFontFamily), typeof(string), typeof(CartesianChart), null, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty LegendFontSizeProperty =
            BindableProperty.Create(
                nameof(LegendFontSize), typeof(double), typeof(CartesianChart), 13d, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty LegendTextColorProperty =
            BindableProperty.Create(
                nameof(LegendTextColor), typeof(c), typeof(CartesianChart),
                new c(35 / 255d, 35 / 255d, 35 / 255d), propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty LegendBackgroundProperty =
            BindableProperty.Create(
                nameof(LegendTextColor), typeof(c), typeof(CartesianChart),
                new c(250 / 255d, 250 / 255d, 250 / 255d), propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty LegendFontAttributesProperty =
            BindableProperty.Create(
                nameof(LegendFontAttributes), typeof(FontAttributes), typeof(CartesianChart),
                FontAttributes.None, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty TooltipPositionProperty =
           BindableProperty.Create(
               nameof(TooltipPosition), typeof(TooltipPosition), typeof(CartesianChart),
               LiveCharts.CurrentSettings.DefaultTooltipPosition, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty TooltipFindingStrategyProperty =
            BindableProperty.Create(
                nameof(TooltipFindingStrategy), typeof(TooltipFindingStrategy), typeof(CartesianChart),
                LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy);

        public static readonly BindableProperty TooltipTemplateProperty =
            BindableProperty.Create(
                nameof(TooltipTemplate), typeof(DataTemplate), typeof(CartesianChart), null, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty TooltipFontFamilyProperty =
            BindableProperty.Create(
                nameof(TooltipFontFamily), typeof(string), typeof(CartesianChart), null, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty TooltipFontSizeProperty =
            BindableProperty.Create(
                nameof(TooltipFontSize), typeof(double), typeof(CartesianChart), 13d, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty TooltipTextColorProperty =
            BindableProperty.Create(
                nameof(TooltipTextColor), typeof(c), typeof(CartesianChart),
                new c(35 / 255d, 35 / 255d, 35 / 255d), propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty TooltipBackgroundProperty =
            BindableProperty.Create(
                nameof(TooltipTextColor), typeof(c), typeof(CartesianChart),
                new c(250 / 255d, 250 / 255d, 250 / 255d), propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty TooltipFontAttributesProperty =
            BindableProperty.Create(
                nameof(TooltipFontAttributes), typeof(FontAttributes), typeof(CartesianChart),
                FontAttributes.None, propertyChanged: OnBindablePropertyChanged);

        #endregion

        #region properties

        CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core
        {
            get
            {
                if (core == null) throw new Exception("core not found");
                return (CartesianChart<SkiaSharpDrawingContext>)core;
            }
        }

        SizeF IChartView.ControlSize
        {
            get
            {
                return new SizeF
                {
                    Width = (float)(Width * DeviceDisplay.MainDisplayInfo.Density),
                    Height = (float)(Height * DeviceDisplay.MainDisplayInfo.Density)
                };
            }
        }

        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => canvas.CanvasCore;

        Grid IMobileChart.LayoutGrid => grid ??= this.FindByName<Grid>("gridLayout");

        BindableObject IMobileChart.Canvas => canvas;

        BindableObject IMobileChart.Legend => legend;

        public Margin? DrawMargin
        {
            get { return (Margin)GetValue(DrawMarginProperty); }
            set { SetValue(DrawMarginProperty, value); }
        }

        public IEnumerable<ISeries> Series
        {
            get { return (IEnumerable<ISeries>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public IEnumerable<IAxis> XAxes
        {
            get { return (IEnumerable<IAxis>)GetValue(XAxesProperty); }
            set { SetValue(XAxesProperty, value); }
        }

        public IEnumerable<IAxis> YAxes
        {
            get { return (IEnumerable<IAxis>)GetValue(YAxesProperty); }
            set { SetValue(YAxesProperty, value); }
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

        public ZoomAndPanMode ZoomMode
        {
            get { return (ZoomAndPanMode)GetValue(ZoomModeProperty); }
            set { SetValue(ZoomModeProperty, value); }
        }

        public double ZoomingSpeed
        {
            get { return (double)GetValue(ZoomingSpeedProperty); }
            set { SetValue(ZoomingSpeedProperty, value); }
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

        public DataTemplate LegendTemplate
        {
            get { return (DataTemplate)GetValue(LegendTemplateProperty); }
            set { SetValue(LegendTemplateProperty, value); }
        }

        public string LegendFontFamily
        {
            get { return (string)GetValue(LegendFontFamilyProperty); }
            set { SetValue(LegendFontFamilyProperty, value); }
        }

        public double LegendFontSize
        {
            get { return (double)GetValue(LegendFontSizeProperty); }
            set { SetValue(LegendFontSizeProperty, value); }
        }

        public c LegendTextColor
        {
            get { return (c)GetValue(LegendTextColorProperty); }
            set { SetValue(LegendTextColorProperty, value); }
        }

        public c LegendBackgroundColor
        {
            get { return (c)GetValue(LegendBackgroundProperty); }
            set { SetValue(LegendBackgroundProperty, value); }
        }

        public FontAttributes LegendFontAttributes
        {
            get { return (FontAttributes)GetValue(LegendFontAttributesProperty); }
            set { SetValue(LegendFontAttributesProperty, value); }
        }

        public IChartLegend<SkiaSharpDrawingContext>? Legend => legend;

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

        public DataTemplate TooltipTemplate
        {
            get { return (DataTemplate)GetValue(TooltipTemplateProperty); }
            set { SetValue(TooltipTemplateProperty, value); }
        }

        public string TooltipFontFamily
        {
            get { return (string)GetValue(TooltipFontFamilyProperty); }
            set { SetValue(TooltipFontFamilyProperty, value); }
        }

        public double TooltipFontSize
        {
            get { return (double)GetValue(TooltipFontSizeProperty); }
            set { SetValue(TooltipFontSizeProperty, value); }
        }

        public c TooltipTextColor
        {
            get { return (c)GetValue(TooltipTextColorProperty); }
            set { SetValue(TooltipTextColorProperty, value); }
        }

        public c TooltipBackgroundColor
        {
            get { return (c)GetValue(TooltipBackgroundProperty); }
            set { SetValue(TooltipBackgroundProperty, value); }
        }

        public FontAttributes TooltipFontAttributes
        {
            get { return (FontAttributes)GetValue(TooltipFontAttributesProperty); }
            set { SetValue(TooltipFontAttributesProperty, value); }
        }

        public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => tooltip;

        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; } = new();

        #endregion

        public PointF ScaleUIPoint(PointF point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            if (core == null) throw new Exception("core not found");
            var cartesianCore = (CartesianChart<SkiaSharpDrawingContext>)core;
            return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
        }

        protected void InitializeCore()
        {
            core = new CartesianChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);
            MainThread.BeginInvokeOnMainThread(() => core.Update());
        }

        protected void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (core == null) return;
            MainThread.BeginInvokeOnMainThread(() => core.Update());
        }

        protected void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (core == null) return;
            MainThread.BeginInvokeOnMainThread(() => core.Update());
        }

        protected static void OnBindablePropertyChanged(BindableObject o, object oldValue, object newValue)
        {
            var chart = (CartesianChart)o;
            if (chart.core == null) return;
            MainThread.BeginInvokeOnMainThread(() => chart.core.Update());
        }

        private void OnSizeChanged(object? sender, EventArgs e)
        {
            if (core == null) return;
            MainThread.BeginInvokeOnMainThread(() => core.Update());
        }

        private void PanGestureRecognizer_PanUpdated(object? sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType != GestureStatus.Running || core == null) return;

            var c = (CartesianChart<SkiaSharpDrawingContext>)core;
            c.Pan(new PointF((float)e.TotalX, (float)e.TotalY));
        }

        private void PinchGestureRecognizer_PinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
        {
            Trace.WriteLine($"{e.ScaleOrigin.X}, {e.ScaleOrigin.Y}");
            if (e.Status != GestureStatus.Running || Math.Abs(e.Scale - 1) < 0.05 || core == null) return;

            var c = (CartesianChart<SkiaSharpDrawingContext>)core;
            var p = e.ScaleOrigin;
            var s = c.ControlSize;

            c.Zoom(
                new PointF((float)(p.X * s.Width), (float)(p.Y * s.Height)),
                e.Scale > 1 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
        }

        private void OnSkCanvasTouched(object? sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            if (TooltipPosition == TooltipPosition.Hidden || core == null) return;
            var location = new PointF(e.Location.X, e.Location.Y);
            ((IChartTooltip<SkiaSharpDrawingContext>)tooltip).Show(core.FindPointsNearTo(location), core);
        }
    }
}
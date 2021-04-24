// The MIT License(MIT)

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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Collections.ObjectModel;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using A = Avalonia;

namespace LiveChartsCore.SkiaSharp.Avalonia
{
    public class PieChart : UserControl, IPieChartView<SkiaSharpDrawingContext>, IAvaloniaChart
    {
        #region fields

        protected Chart<SkiaSharpDrawingContext>? core;
        protected IChartLegend<SkiaSharpDrawingContext>? legend;
        protected IChartTooltip<SkiaSharpDrawingContext>? tooltip;
        private readonly ActionThrottler mouseMoveThrottler;
        private PointF mousePosition = new();
        private readonly CollectionDeepObserver<ISeries> seriesObserver;

        #endregion

        public PieChart()
        {
            InitializeComponent();

            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ConstructChart(this);

            InitializeCore();

            mouseMoveThrottler = new ActionThrottler(MouseMoveThrottlerUnlocked, TimeSpan.FromMilliseconds(10));

            seriesObserver = new CollectionDeepObserver<ISeries>(
               (object? sender, NotifyCollectionChangedEventArgs e) =>
               {
                   if (core == null) return;
                   Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
               },
               (object? sender, PropertyChangedEventArgs e) =>
               {
                   if (core == null) return;
                   Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
               });

            Series = new ObservableCollection<ISeries>();

            // workaround to deteck mouse events.
            // avalonia do not seem to detect events if brackground is not set.
            Background = new SolidColorBrush(Colors.Transparent);

            PointerMoved += CartesianChart_PointerMoved;
        }

        #region avalonia/dependency properties

        public static readonly AvaloniaProperty<Margin?> DrawMarginProperty =
           AvaloniaProperty.Register<CartesianChart, Margin?>(nameof(DrawMargin), null, inherits: true);

        public static readonly AvaloniaProperty<IEnumerable<ISeries>> SeriesProperty =
            AvaloniaProperty.Register<PieChart, IEnumerable<ISeries>>(nameof(Series), new List<ISeries>(), inherits: true);

        public static readonly AvaloniaProperty<TimeSpan> AnimationsSpeedProperty =
            AvaloniaProperty.Register<PieChart, TimeSpan>(
                nameof(AnimationsSpeed), LiveCharts.CurrentSettings.DefaultAnimationsSpeed, inherits: true);

        public static readonly AvaloniaProperty<Func<float, float>> EasingFunctionProperty =
            AvaloniaProperty.Register<PieChart, Func<float, float>>(
                nameof(AnimationsSpeed), LiveCharts.CurrentSettings.DefaultEasingFunction, inherits: true);

        public static readonly AvaloniaProperty<DataTemplate?> TooltipTemplateProperty =
            AvaloniaProperty.Register<CartesianChart, DataTemplate?>(nameof(TooltipTemplate), null, inherits: true);

        public static readonly AvaloniaProperty<TooltipPosition> TooltipPositionProperty =
            AvaloniaProperty.Register<CartesianChart, TooltipPosition>(
                nameof(TooltipPosition), LiveCharts.CurrentSettings.DefaultTooltipPosition, inherits: true);

        public static readonly AvaloniaProperty<TooltipFindingStrategy> TooltipFindingStrategyProperty =
            AvaloniaProperty.Register<CartesianChart, TooltipFindingStrategy>(
                nameof(LegendPosition), LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy, inherits: true);

        public static readonly AvaloniaProperty<A.Media.FontFamily> TooltipFontFamilyProperty =
            AvaloniaProperty.Register<CartesianChart, A.Media.FontFamily>(
                nameof(TooltipFontFamily), new A.Media.FontFamily("Arial"), inherits: true);

        public static readonly AvaloniaProperty<double> TooltipFontSizeProperty =
            AvaloniaProperty.Register<CartesianChart, double>(nameof(TooltipFontSize), 13d, inherits: true);

        public static readonly AvaloniaProperty<FontWeight> TooltipFontWeightProperty =
            AvaloniaProperty.Register<CartesianChart, FontWeight>(nameof(TooltipFontWeight), FontWeight.Normal, inherits: true);

        public static readonly AvaloniaProperty<A.Media.FontStyle> TooltipFontStyleProperty =
            AvaloniaProperty.Register<CartesianChart, A.Media.FontStyle>(
                nameof(TooltipFontStyle), A.Media.FontStyle.Normal, inherits: true);

        public static readonly AvaloniaProperty<SolidColorBrush> TooltipTextBrushProperty =
            AvaloniaProperty.Register<CartesianChart, SolidColorBrush>(
                nameof(TooltipTextBrush), new SolidColorBrush(new A.Media.Color(255, 35, 35, 35)), inherits: true);

        public static readonly AvaloniaProperty<IBrush> TooltipBackgroundProperty =
            AvaloniaProperty.Register<CartesianChart, IBrush>(nameof(TooltipBackground),
                new SolidColorBrush(new A.Media.Color(255, 250, 250, 250)), inherits: true);

        public static readonly AvaloniaProperty<LegendPosition> LegendPositionProperty =
            AvaloniaProperty.Register<CartesianChart, LegendPosition>(
                nameof(LegendPosition), LiveCharts.CurrentSettings.DefaultLegendPosition, inherits: true);

        public static readonly AvaloniaProperty<LegendOrientation> LegendOrientationProperty =
            AvaloniaProperty.Register<CartesianChart, LegendOrientation>(
                nameof(LegendOrientation), LiveCharts.CurrentSettings.DefaultLegendOrientation, inherits: true);

        public static readonly AvaloniaProperty<DataTemplate?> LegendTemplateProperty =
            AvaloniaProperty.Register<CartesianChart, DataTemplate?>(nameof(LegendTemplate), null, inherits: true);

        public static readonly AvaloniaProperty<A.Media.FontFamily> LegendFontFamilyProperty =
           AvaloniaProperty.Register<CartesianChart, A.Media.FontFamily>(
               nameof(LegendFontFamily), new A.Media.FontFamily("Arial"), inherits: true);

        public static readonly AvaloniaProperty<double> LegendFontSizeProperty =
            AvaloniaProperty.Register<CartesianChart, double>(nameof(LegendFontSize), 13d, inherits: true);

        public static readonly AvaloniaProperty<FontWeight> LegendFontWeightProperty =
            AvaloniaProperty.Register<CartesianChart, FontWeight>(nameof(LegendFontWeight), FontWeight.Normal, inherits: true);

        public static readonly AvaloniaProperty<A.Media.FontStyle> LegendFontStyleProperty =
            AvaloniaProperty.Register<CartesianChart, A.Media.FontStyle>(
                nameof(LegendFontStyle), A.Media.FontStyle.Normal, inherits: true);

        public static readonly AvaloniaProperty<SolidColorBrush> LegendTextBrushProperty =
            AvaloniaProperty.Register<CartesianChart, SolidColorBrush>(
                nameof(LegendTextBrush), new SolidColorBrush(new A.Media.Color(255, 35, 35, 35)), inherits: true);

        public static readonly AvaloniaProperty<IBrush> LegendBackgroundProperty =
            AvaloniaProperty.Register<CartesianChart, IBrush>(nameof(LegendBackground),
                new SolidColorBrush(new A.Media.Color(255, 250, 250, 250)), inherits: true);

        #endregion

        SizeF IChartView.ControlSize
        {
            get
            {
                return new SizeF
                {
                    Width = (float)Bounds.Width,
                    Height = (float)Bounds.Height
                };
            }
        }

        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas
        {
            get
            {
                if (core == null) throw new Exception("core not found");
                return core.Canvas;
            }
        }

        PieChart<SkiaSharpDrawingContext> IPieChartView<SkiaSharpDrawingContext>.Core
        {
            get
            {
                if (core == null) throw new Exception("core not found");
                return (PieChart<SkiaSharpDrawingContext>)core;
            }
        }

        public Margin? DrawMargin
        {
            get { return (Margin?)GetValue(DrawMarginProperty); }
            set { SetValue(DrawMarginProperty, value); }
        }

        public IEnumerable<ISeries> Series
        {
            get { return (IEnumerable<ISeries>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
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

        public A.Media.FontFamily TooltipFontFamily
        {
            get { return (A.Media.FontFamily)GetValue(TooltipFontFamilyProperty); }
            set { SetValue(TooltipFontFamilyProperty, value); }
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

        public A.Media.FontStyle TooltipFontStyle
        {
            get { return (A.Media.FontStyle)GetValue(TooltipFontStyleProperty); }
            set { SetValue(TooltipFontStyleProperty, value); }
        }

        public SolidColorBrush TooltipTextBrush
        {
            get { return (SolidColorBrush)GetValue(TooltipTextBrushProperty); }
            set { SetValue(TooltipTextBrushProperty, value); }
        }

        public IBrush TooltipBackground
        {
            get { return (IBrush)GetValue(TooltipBackgroundProperty); }
            set { SetValue(TooltipBackgroundProperty, value); }
        }

        public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => tooltip;

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

        public A.Media.FontFamily LegendFontFamily
        {
            get { return (A.Media.FontFamily)GetValue(LegendFontFamilyProperty); }
            set { SetValue(LegendFontFamilyProperty, value); }
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

        public A.Media.FontStyle LegendFontStyle
        {
            get { return (A.Media.FontStyle)GetValue(LegendFontStyleProperty); }
            set { SetValue(LegendFontStyleProperty, value); }
        }
        public SolidColorBrush LegendTextBrush
        {
            get { return (SolidColorBrush)GetValue(LegendTextBrushProperty); }
            set { SetValue(LegendTextBrushProperty, value); }
        }

        public IBrush LegendBackground
        {
            get { return (IBrush)GetValue(LegendBackgroundProperty); }
            set { SetValue(LegendBackgroundProperty, value); }
        }

        public IChartLegend<SkiaSharpDrawingContext>? Legend => legend;

        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; } = new();

        protected void InitializeCore()
        {
            var canvas = this.FindControl<MotionCanvas>("canvas");
            core = new PieChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);
            //legend = Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
            //tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaSharpDrawingContext>;
            Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (core == null) throw new Exception("core not found");

            if (change.Property.Name == nameof(Series))
            {
                seriesObserver.Dispose((IEnumerable<ISeries>)change.OldValue.Value);
                seriesObserver.Initialize((IEnumerable<ISeries>)change.NewValue.Value);
                return;
            }

            Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
        }

        private void MouseMoveThrottlerUnlocked()
        {
            if (core == null || tooltip == null || TooltipPosition == TooltipPosition.Hidden) return;
            tooltip.Show(core.FindPointsNearTo(mousePosition), core);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void CartesianChart_PointerMoved(object? sender, global::Avalonia.Input.PointerEventArgs e)
        {
            var p = e.GetPosition(this);
            mousePosition = new PointF((float)p.X, (float)p.Y);
            mouseMoveThrottler.Call();
        }
    }
}

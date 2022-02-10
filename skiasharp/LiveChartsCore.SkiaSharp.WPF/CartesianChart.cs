﻿// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}" />
    public class CartesianChart : Chart, ICartesianChartView<SkiaSharpDrawingContext>
    {
        #region fields

        private readonly CollectionDeepObserver<ISeries> _seriesObserver;
        private readonly CollectionDeepObserver<IAxis> _xObserver;
        private readonly CollectionDeepObserver<IAxis> _yObserver;
        private readonly ActionThrottler _panningThrottler;
        private System.Windows.Point? _previous;
        private System.Windows.Point? _current;
        private bool _isPanning = false;

        #endregion

        static CartesianChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CartesianChart), new FrameworkPropertyMetadata(typeof(CartesianChart)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianChart"/> class.
        /// </summary>
        public CartesianChart()
        {
            _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            _xObserver = new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            _yObserver = new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

            SetCurrentValue(XAxesProperty, new List<IAxis>() { new Axis() });
            SetCurrentValue(YAxesProperty, new List<IAxis>() { new Axis() });
            SetCurrentValue(SeriesProperty, new ObservableCollection<ISeries>());

            MouseWheel += OnMouseWheel;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;

            _panningThrottler = new ActionThrottler(DoPan, TimeSpan.FromMilliseconds(30));
        }

        #region dependency properties

        /// <summary>
        /// The series property
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(Series), typeof(IEnumerable<ISeries>), typeof(CartesianChart), new PropertyMetadata(null,
                    (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                    {
                        var chart = (CartesianChart)o;
                        var seriesObserver = chart._seriesObserver;
                        seriesObserver.Dispose((IEnumerable<ISeries>)args.OldValue);
                        seriesObserver.Initialize((IEnumerable<ISeries>)args.NewValue);
                        if (chart.core == null) return;
                        Application.Current.Dispatcher.Invoke(() => chart.core.Update());
                    },
                    (DependencyObject o, object value) =>
                    {
                        return value is IEnumerable<ISeries> ? value : new ObservableCollection<ISeries>();
                    }));

        /// <summary>
        /// The x axes property
        /// </summary>
        public static readonly DependencyProperty XAxesProperty =
            DependencyProperty.Register(
                nameof(XAxes), typeof(IEnumerable<IAxis>), typeof(CartesianChart), new PropertyMetadata(null,
                    (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                    {
                        var chart = (CartesianChart)o;
                        var observer = chart._xObserver;
                        observer.Dispose((IEnumerable<IAxis>)args.OldValue);
                        observer.Initialize((IEnumerable<IAxis>)args.NewValue);
                        if (chart.core == null) return;
                        Application.Current.Dispatcher.Invoke(() => chart.core.Update());
                    },
                    (DependencyObject o, object value) =>
                    {
                        return value is IEnumerable<IAxis> ? value : new List<IAxis>() { new Axis() };
                    }));

        /// <summary>
        /// The y axes property
        /// </summary>
        public static readonly DependencyProperty YAxesProperty =
            DependencyProperty.Register(
                nameof(YAxes), typeof(IEnumerable<IAxis>), typeof(CartesianChart), new PropertyMetadata(null,
                    (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                    {
                        var chart = (CartesianChart)o;
                        var observer = chart._yObserver;
                        observer.Dispose((IEnumerable<IAxis>)args.OldValue);
                        observer.Initialize((IEnumerable<IAxis>)args.NewValue);
                        if (chart.core == null) return;
                        Application.Current.Dispatcher.Invoke(() => chart.core.Update());
                    },
                    (DependencyObject o, object value) =>
                    {
                        return value is IEnumerable<IAxis> ? value : new List<IAxis>() { new Axis() };
                    }));

        /// <summary>
        /// The zoom mode property
        /// </summary>
        public static readonly DependencyProperty ZoomModeProperty =
            DependencyProperty.Register(
                nameof(ZoomMode), typeof(ZoomAndPanMode), typeof(CartesianChart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultZoomMode));

        /// <summary>
        /// The zooming speed property
        /// </summary>
        public static readonly DependencyProperty ZoomingSpeedProperty =
            DependencyProperty.Register(
                nameof(ZoomingSpeed), typeof(double), typeof(CartesianChart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultZoomSpeed));

        #endregion

        #region properties

        CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core =>
            core == null ? throw new Exception("core not found") : (CartesianChart<SkiaSharpDrawingContext>)core;

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Series" />
        public IEnumerable<ISeries> Series
        {
            get => (IEnumerable<ISeries>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.XAxes" />
        public IEnumerable<IAxis> XAxes
        {
            get => (IEnumerable<IAxis>)GetValue(XAxesProperty);
            set => SetValue(XAxesProperty, value);
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.YAxes" />
        public IEnumerable<IAxis> YAxes
        {
            get => (IEnumerable<IAxis>)GetValue(YAxesProperty);
            set => SetValue(YAxesProperty, value);
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomMode" />
        public ZoomAndPanMode ZoomMode
        {
            get => (ZoomAndPanMode)GetValue(ZoomModeProperty);
            set => SetValue(ZoomModeProperty, value);
        }

        ZoomAndPanMode ICartesianChartView<SkiaSharpDrawingContext>.ZoomMode
        {
            get => ZoomMode;
            set => SetValueOrCurrentValue(ZoomModeProperty, value);
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomingSpeed" />
        public double ZoomingSpeed
        {
            get => (double)GetValue(ZoomingSpeedProperty);
            set => SetValue(ZoomingSpeedProperty, value);
        }

        double ICartesianChartView<SkiaSharpDrawingContext>.ZoomingSpeed
        {
            get => ZoomingSpeed;
            set => SetValueOrCurrentValue(ZoomingSpeedProperty, value);
        }

        #endregion

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScaleUIPoint(PointF, int, int)" />
        public PointF ScaleUIPoint(PointF point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            if (core == null) throw new Exception("core not found");
            var cartesianCore = (CartesianChart<SkiaSharpDrawingContext>)core;
            return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
        }

        /// <summary>
        /// Initializes the core.
        /// </summary>
        /// <exception cref="Exception">canvas not found</exception>
        protected override void InitializeCore()
        {
            if (canvas == null) throw new Exception("canvas not found");

            core = new CartesianChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);
            legend = Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
            tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaSharpDrawingContext>;
            Application.Current.Dispatcher.Invoke(() => core.Update());
        }

        private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (core == null) return;
            Application.Current.Dispatcher.Invoke(() => core.Update());
        }

        private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (core == null) return;
            Application.Current.Dispatcher.Invoke(() => core.Update());
        }

        private void OnMouseWheel(object? sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (core == null) throw new Exception("core not found");
            var c = (CartesianChart<SkiaSharpDrawingContext>)core;
            var p = e.GetPosition(this);
            c.Zoom(new PointF((float)p.X, (float)p.Y), e.Delta > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
        }

        private void OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isPanning = true;
            _previous = e.GetPosition(this);
            _ = CaptureMouse();
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_isPanning || _previous == null) return;

            _current = e.GetPosition(this);
            _panningThrottler.Call();
        }

        private void DoPan()
        {
            if (core == null) throw new Exception("core not found");
            if (_previous == null || _current == null) return;

            var c = (CartesianChart<SkiaSharpDrawingContext>)core;

            c.Pan(
                new PointF(
                (float)(_current.Value.X - _previous.Value.X),
                (float)(_current.Value.Y - _previous.Value.Y)));

            _previous = new System.Windows.Point(_current.Value.X, _current.Value.Y);
        }

        private void OnMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!_isPanning) return;
            _isPanning = false;
            _previous = null;
            ReleaseMouseCapture();
        }
    }
}


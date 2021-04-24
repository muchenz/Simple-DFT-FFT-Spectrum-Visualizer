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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LiveChartsCore.Measure;

namespace LiveChartsCore
{
    public class CartesianChart<TDrawingContext> : Chart<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        internal readonly HashSet<ISeries> everMeasuredSeries = new();
        internal readonly HashSet<IAxis<TDrawingContext>> everMeasuredAxes = new();
        private readonly ICartesianChartView<TDrawingContext> chartView;
        private int nextSeries = 0;
        private IAxis<TDrawingContext>[] secondaryAxes = new IAxis<TDrawingContext>[0];
        private IAxis<TDrawingContext>[] primaryAxes = new IAxis<TDrawingContext>[0];
        private double zoomingSpeed = 0;
        private ZoomAndPanMode zoomMode;
        private ICartesianSeries<TDrawingContext>[] series = new ICartesianSeries<TDrawingContext>[0];

        public CartesianChart(
            ICartesianChartView<TDrawingContext> view,
            Action<LiveChartsSettings> defaultPlatformConfig,
            MotionCanvas<TDrawingContext> canvas)
            : base(canvas, defaultPlatformConfig)
        {
            chartView = view;

            view.PointStates.Chart = this;
            foreach (var item in view.PointStates.GetStates())
            {
                if (item.Fill != null)
                {
                    item.Fill.ZIndex += 1000000;
                    canvas.AddDrawableTask(item.Fill);
                }
                if (item.Stroke != null)
                {
                    item.Stroke.ZIndex += 1000000;
                    canvas.AddDrawableTask(item.Stroke);
                }
            }
        }

        public object Sync = new();
        public IAxis<TDrawingContext>[] XAxes => secondaryAxes;
        public IAxis<TDrawingContext>[] YAxes => primaryAxes;
        public ICartesianSeries<TDrawingContext>[] Series => series;
        public override IEnumerable<IDrawableSeries<TDrawingContext>> DrawableSeries => series;
        public override IChartView<TDrawingContext> View => chartView;

        public override void Update(bool throttling = true)
        {
            if (!throttling)
            {
                updateThrottler.ForceCall();
                return;
            }

            updateThrottler.Call();
        }

        public override IEnumerable<TooltipPoint> FindPointsNearTo(PointF pointerPosition)
        {
            if (measureWorker == null) return Enumerable.Empty<TooltipPoint>();

            return chartView.Series.SelectMany(series => series.FindPointsNearTo(this, pointerPosition));
        }

        public PointF ScaleUIPoint(PointF point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            var xAxis = secondaryAxes[xAxisIndex];
            var yAxis = primaryAxes[yAxisIndex];

            var xScaler = new Scaler(drawMaringLocation, drawMarginSize, xAxis);
            var yScaler = new Scaler(drawMaringLocation, drawMarginSize, yAxis);

            return new PointF(xScaler.ToChartValues(point.X), yScaler.ToChartValues(point.Y));
        }

        public void Zoom(PointF pivot, ZoomDirection direction)
        {
            if (primaryAxes == null || secondaryAxes == null) return;

            var speed = zoomingSpeed < 0.1 ? 0.1 : (zoomingSpeed > 0.95 ? 0.95 : zoomingSpeed);
            var m = direction == ZoomDirection.ZoomIn ? speed : 1 / speed;

            if ((zoomMode & ZoomAndPanMode.X) == ZoomAndPanMode.X)
            {
                for (var index = 0; index < secondaryAxes.Length; index++)
                {
                    var xi = secondaryAxes[index];
                    var px = new Scaler(drawMaringLocation, drawMarginSize, xi).ToChartValues(pivot.X);

                    var max = xi.MaxLimit == null ? xi.DataBounds.Max : xi.MaxLimit.Value;
                    var min = xi.MinLimit == null ? xi.DataBounds.Min : xi.MinLimit.Value;

                    var l = max - min;

                    var rMin = (px - min) / l;
                    var rMax = 1 - rMin;

                    var target = l * m;
                    //if (target < xi.View.MinRange) return;
                    var mint = px - target * rMin;
                    var maxt = px + target * rMax;

                    if (maxt > xi.DataBounds.Max) maxt = xi.DataBounds.Max;
                    if (mint < xi.DataBounds.Min) mint = xi.DataBounds.Min;

                    xi.MaxLimit = maxt;
                    xi.MinLimit = mint;
                }
            }

            if ((zoomMode & ZoomAndPanMode.Y) == ZoomAndPanMode.Y)
            {
                for (var index = 0; index < primaryAxes.Length; index++)
                {
                    var yi = primaryAxes[index];
                    var px = new Scaler(drawMaringLocation, drawMarginSize, yi).ToChartValues(pivot.X);

                    var max = yi.MaxLimit == null ? yi.DataBounds.Max : yi.MaxLimit.Value;
                    var min = yi.MinLimit == null ? yi.DataBounds.Min : yi.MinLimit.Value;

                    var l = max - min;

                    var rMin = (px - min) / l;
                    var rMax = 1 - rMin;

                    var target = l * m;
                    //if (target < xi.View.MinRange) return;
                    var mint = px - target * rMin;
                    var maxt = px + target * rMax;

                    if (maxt > yi.DataBounds.Max) maxt = yi.DataBounds.Max;
                    if (mint < yi.DataBounds.Min) mint = yi.DataBounds.Min;

                    yi.MaxLimit = maxt;
                    yi.MinLimit = mint;
                }
            }
        }

        public void Pan(PointF delta)
        {
            if ((zoomMode & ZoomAndPanMode.X) == ZoomAndPanMode.X)
            {
                for (var index = 0; index < secondaryAxes.Length; index++)
                {
                    var xi = secondaryAxes[index];
                    var scale = new Scaler(drawMaringLocation, drawMarginSize, xi);
                    var dx = scale.ToChartValues(-delta.X) - scale.ToChartValues(0);

                    var max = xi.MaxLimit == null ? xi.DataBounds.Max : xi.MaxLimit.Value;
                    var min = xi.MinLimit == null ? xi.DataBounds.Min : xi.MinLimit.Value;

                    xi.MaxLimit = max + dx > xi.DataBounds.Max ? xi.DataBounds.Max : max + dx;
                    xi.MinLimit = min + dx < xi.DataBounds.Min ? xi.DataBounds.Min : min + dx;
                }
            }

            if ((zoomMode & ZoomAndPanMode.Y) == ZoomAndPanMode.Y)
            {
                for (var index = 0; index < primaryAxes.Length; index++)
                {
                    var yi = secondaryAxes[index];
                    var scale = new Scaler(drawMaringLocation, drawMarginSize, yi);
                    var dy = scale.ToChartValues(delta.Y) - scale.ToChartValues(0);

                    var max = yi.MaxLimit == null ? yi.DataBounds.Max : yi.MaxLimit.Value;
                    var min = yi.MinLimit == null ? yi.DataBounds.Min : yi.MinLimit.Value;

                    yi.MaxLimit = max + dy > yi.DataBounds.Max ? yi.DataBounds.Max : max + dy;
                    yi.MinLimit = min + dy < yi.DataBounds.Min ? yi.DataBounds.Min : min + dy;
                }
            }
        }

        protected override void Measure()
        {
            seriesContext = new SeriesContext<TDrawingContext>(series);

            Canvas.MeasuredDrawables = new HashSet<IDrawable<TDrawingContext>>();
            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<TDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");

            lock (canvas.Sync)
            {
                // restart axes bounds and meta data
                foreach (var axis in secondaryAxes)
                {
                    axis.Initialize(AxisOrientation.X);
                    initializer.ResolveAxisDefaults(axis);
                }
                foreach (var axis in primaryAxes)
                {
                    axis.Initialize(AxisOrientation.Y);
                    initializer.ResolveAxisDefaults(axis);
                }

                // get seriesBounds
                foreach (var series in series)
                {
                    if (series.SeriesId == -1) series.SeriesId = nextSeries++;
                    initializer.ResolveSeriesDefaults(stylesBuilder.CurrentColors, series);

                    var secondaryAxis = secondaryAxes[series.ScalesXAt];
                    var primaryAxis = primaryAxes[series.ScalesYAt];

                    var seriesBounds = series.GetBounds(this, secondaryAxis, primaryAxis);

                    secondaryAxis.DataBounds.AppendValue(seriesBounds.SecondaryBounds.max);
                    secondaryAxis.DataBounds.AppendValue(seriesBounds.SecondaryBounds.min);
                    secondaryAxis.VisibleDataBounds.AppendValue(seriesBounds.VisibleSecondaryBounds.max);
                    secondaryAxis.VisibleDataBounds.AppendValue(seriesBounds.VisibleSecondaryBounds.min);
                    primaryAxis.DataBounds.AppendValue(seriesBounds.PrimaryBounds.max);
                    primaryAxis.DataBounds.AppendValue(seriesBounds.PrimaryBounds.min);
                    primaryAxis.VisibleDataBounds.AppendValue(seriesBounds.VisiblePrimaryBounds.max);
                    primaryAxis.VisibleDataBounds.AppendValue(seriesBounds.VisiblePrimaryBounds.min);
                }

                if (legend != null) legend.Draw(this);

                if (viewDrawMargin == null)
                {
                    var m = viewDrawMargin ?? new Margin();
                    float ts = 0f, bs = 0f, ls = 0f, rs = 0f;
                    SetDrawMargin(controlSize, m);

                    foreach (var axis in secondaryAxes)
                    {
                        var s = axis.GetPossibleSize(this);
                        if (axis.Position == AxisPosition.Start)
                        {
                            // X Bottom
                            axis.Yo = m.Bottom + s.Height * 0.5f;
                            bs = bs + s.Height;
                            m.Bottom = bs;
                            //if (s.Width * 0.5f > m.Left) m.Left = s.Width * 0.5f;
                            //if (s.Width * 0.5f > m.Right) m.Right = s.Width * 0.5f;
                        }
                        else
                        {
                            // X Top
                            axis.Yo = ts + s.Height * 0.5f;
                            ts += s.Height;
                            m.Top = ts;
                            //if (ls + s.Width * 0.5f > m.Left) m.Left = ls + s.Width * 0.5f;
                            //if (rs + s.Width * 0.5f > m.Right) m.Right = rs + s.Width * 0.5f;
                        }
                    }
                    foreach (var axis in primaryAxes)
                    {
                        var s = axis.GetPossibleSize(this);
                        var w = s.Width > m.Left ? s.Width : m.Left;
                        if (axis.Position == AxisPosition.Start)
                        {
                            // Y Left
                            axis.Xo = ls + w * 0.5f;
                            ls += w;
                            m.Left = ls;
                            //if (s.Height * 0.5f > m.Top) { m.Top = s.Height * 0.5f; }
                            //if (s.Height * 0.5f > m.Bottom) { m.Bottom = s.Height * 0.5f; }
                        }
                        else
                        {
                            // Y Right
                            axis.Xo = rs + w * 0.5f;
                            rs += w;
                            m.Right = rs;
                            //if (ts + s.Height * 0.5f > m.Top) m.Top = ts + s.Height * 0.5f;
                            //if (bs + s.Height * 0.5f > m.Bottom) m.Bottom = bs + s.Height * 0.5f;
                        }
                    }

                    SetDrawMargin(controlSize, m);
                }

                // invalid dimensions, probably the chart is too small
                // or it is initializing in the UI and has no dimensions yet
                if (drawMarginSize.Width <= 0 || drawMarginSize.Height <= 0) return;

                var totalAxes = primaryAxes.Concat(secondaryAxes).ToArray();
                var toDeleteAxes = new HashSet<IAxis<TDrawingContext>>(everMeasuredAxes);
                foreach (var axis in totalAxes)
                {
                    if (axis.DataBounds.Max == axis.DataBounds.Min)
                    {
                        var c = axis.DataBounds.Min * 0.3;
                        axis.DataBounds.Min = axis.DataBounds.Min - c;
                        axis.DataBounds.Max = axis.DataBounds.Max + c;
                    }

                    axis.Measure(this);
                    everMeasuredAxes.Add(axis);
                    toDeleteAxes.Remove(axis);

                    var deleted = false;
                    foreach (var item in axis.DeletingTasks)
                    {
                        canvas.RemovePaintTask(item);
                        item.Dispose();
                        deleted = true;
                    }
                    if (deleted) axis.DeletingTasks.Clear();
                }

                var toDeleteSeries = new HashSet<ISeries>(everMeasuredSeries);

                foreach (var series in series)
                {
                    var secondaryAxis = secondaryAxes[series.ScalesXAt];
                    var primaryAxis = primaryAxes[series.ScalesYAt];
                    series.Measure(this, secondaryAxis, primaryAxis);
                    everMeasuredSeries.Add(series);
                    toDeleteSeries.Remove(series);

                    var deleted = false;
                    foreach (var item in series.DeletingTasks)
                    {
                        canvas.RemovePaintTask(item);
                        item.Dispose();
                        deleted = true;
                    }
                    if (deleted) series.DeletingTasks.Clear();
                }

                foreach (var series in toDeleteSeries)
                {
                    series.Dispose();
                    everMeasuredSeries.Remove(series);
                }
                foreach (var axis in toDeleteAxes)
                {
                    axis.Dispose();
                    everMeasuredAxes.Remove(axis);
                }
            }

            Canvas.Invalidate();
        }

        protected override void UpdateThrottlerUnlocked()
        {
            // before measure every element in the chart
            // we copy the properties that might change while we are updating the chart
            // this call should be thread safe
            // ToDo: ensure it is thread safe...

            viewDrawMargin = chartView.DrawMargin;
            controlSize = chartView.ControlSize;
            primaryAxes = chartView.YAxes.Cast<IAxis<TDrawingContext>>().Select(x => x).ToArray();
            secondaryAxes = chartView.XAxes.Cast<IAxis<TDrawingContext>>().Select(x => x).ToArray();

            zoomingSpeed = chartView.ZoomingSpeed;
            zoomMode = chartView.ZoomMode;

            measureWorker = new object();
            series = chartView.Series
                .Cast<ICartesianSeries<TDrawingContext>>()
                .Select(series =>
                {
                    series.Fetch(this);
                    return series;
                }).ToArray();

            legendPosition = chartView.LegendPosition;
            legendOrientation = chartView.LegendOrientation;
            legend = chartView.Legend; // ... this is a reference type.. this has no sense

            tooltipPosition = chartView.TooltipPosition;
            tooltipFindingStrategy = chartView.TooltipFindingStrategy;
            tooltip = chartView.Tooltip; //... no sense again...

            animationsSpeed = chartView.AnimationsSpeed;
            easingFunction = chartView.EasingFunction;

            Measure();
        }
    }
}

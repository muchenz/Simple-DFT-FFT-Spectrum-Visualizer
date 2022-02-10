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
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Diagnostics;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines the data to plot as a line.
    /// </summary>
    public class LineSeries<TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TLineSegment, TBezierSegment, TMoveToCommand, TPathArgs>
        : CartesianSeries<TModel, LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>, TLabel, TDrawingContext>, ILineSeries<TDrawingContext>
        where TPathGeometry : IPathGeometry<TDrawingContext, TPathArgs>, new()
        where TLineSegment : ILinePathSegment<TPathArgs>, new()
        where TBezierSegment : IBezierSegment<TPathArgs>, new()
        where TMoveToCommand : IMoveToPathCommand<TPathArgs>, new()
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        private readonly List<AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>> _fillPathHelperContainer = new();
        private readonly List<AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>> _strokePathHelperContainer = new();
        private float _lineSmoothness = 0.65f;
        private float _geometrySize = 14f;
        private bool _enableNullSplitting = true;
        private IDrawableTask<TDrawingContext>? _geometryFill;
        private IDrawableTask<TDrawingContext>? _geometryStroke;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSeries{TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TLineSegment, TBezierSegment, TMoveToCommand, TPathArgs}"/> class.
        /// </summary>
        /// <param name="isStacked">if set to <c>true</c> [is stacked].</param>
        public LineSeries(bool isStacked = false)
            : base(SeriesProperties.Line | SeriesProperties.VerticalOrientation | (isStacked ? SeriesProperties.Stacked : 0))
        {
            DataPadding = new PointF(0.5f, 1f);
            HoverState = LiveCharts.LineSeriesHoverKey;
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometrySize"/>
        public double GeometrySize { get => _geometrySize; set { _geometrySize = (float)value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.LineSmoothness"/>
        public double LineSmoothness
        {
            get => _lineSmoothness;
            set
            {
                var v = value;
                if (value > 1) v = 1;
                if (value < 0) v = 0;
                _lineSmoothness = (float)v;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.EnableNullSplitting"/>
        public bool EnableNullSplitting { get => _enableNullSplitting; set { _enableNullSplitting = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometryFill"/>
        public IDrawableTask<TDrawingContext>? GeometryFill
        {
            get => _geometryFill;
            set
            {
                if (_geometryFill != null) deletingTasks.Add(_geometryFill);
                _geometryFill = value;
                if (_geometryFill != null)
                {
                    _geometryFill.IsStroke = false;
                    _geometryFill.StrokeThickness = 0;
                }

                OnPaintContextChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometrySize"/>
        public IDrawableTask<TDrawingContext>? GeometryStroke
        {
            get => _geometryStroke;
            set
            {
                if (_geometryStroke != null) deletingTasks.Add(_geometryStroke);
                _geometryStroke = value;
                if (_geometryStroke != null)
                {
                    _geometryStroke.IsStroke = true;
                }

                OnPaintContextChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.Measure(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})" />
        public override void Measure(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> xAxis, IAxis<TDrawingContext> yAxis)
        {
            Trace.WriteLine(chart.View.CoreCanvas.CountGeometries());

            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var secondaryScale = new Scaler(drawLocation, drawMarginSize, xAxis);
            var primaryScale = new Scaler(drawLocation, drawMarginSize, yAxis);
            var previousPrimaryScale =
                yAxis.PreviousDataBounds == null ? null : new Scaler(drawLocation, drawMarginSize, yAxis, true);
            var previousSecondaryScale =
                xAxis.PreviousDataBounds == null ? null : new Scaler(drawLocation, drawMarginSize, xAxis, true);

            var gs = _geometrySize;
            var hgs = gs / 2f;
            var sw = Stroke?.StrokeThickness ?? 0;
            var p = primaryScale.ToPixels(pivot);

            var chartAnimation = new Animation(chart.EasingFunction, chart.AnimationsSpeed);

            var fetched = Fetch(chart);
            if (fetched is not ChartPoint[] points) points = fetched.ToArray();

            var segments = _enableNullSplitting
                ? SplitEachNull(points, secondaryScale, primaryScale)
                : new ChartPoint[][] { points };

            var stacker = (SeriesProperties & SeriesProperties.Stacked) == SeriesProperties.Stacked
                ? chart.SeriesContext.GetStackPosition(this, GetStackGroup())
                : null;

            var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;

            if (stacker != null)
            {
                // easy workaround to set an automatic and valid z-index for stacked area series
                // the problem of this solution is that the user needs to set z-indexes above 1000
                // if the user needs to add more series to the chart.
                actualZIndex = 1000 - stacker.Position;
                if (Fill != null) Fill.ZIndex = actualZIndex;
                if (Stroke != null) Stroke.ZIndex = actualZIndex;
            }

            var dls = unchecked((float)DataLabelsSize);

            var segmentI = 0;
            var toDeletePoints = new HashSet<ChartPoint>(everFetched);

            foreach (var segment in segments)
            {
                var wasFillInitialized = false;
                var wasStrokeInitialized = false;

                AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs> fillPathHelper;
                AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs> strokePathHelper;

                if (segmentI >= _fillPathHelperContainer.Count)
                {
                    fillPathHelper = new AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>();
                    strokePathHelper = new AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>();
                    _fillPathHelperContainer.Add(fillPathHelper);
                    _strokePathHelperContainer.Add(strokePathHelper);
                }
                else
                {
                    fillPathHelper = _fillPathHelperContainer[segmentI];
                    strokePathHelper = _strokePathHelperContainer[segmentI];
                }

                if (Fill != null)
                {
                    wasFillInitialized = fillPathHelper.Initialize(SetDefaultPathTransitions, chartAnimation);
                    Fill.AddGeometyToPaintTask(fillPathHelper.Path);
                    chart.Canvas.AddDrawableTask(Fill);
                    Fill.ZIndex = actualZIndex + 0.1;
                    Fill.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                    fillPathHelper.Path.ClearCommands();
                }
                if (Stroke != null)
                {
                    wasStrokeInitialized = strokePathHelper.Initialize(SetDefaultPathTransitions, chartAnimation);
                    Stroke.AddGeometyToPaintTask(strokePathHelper.Path);
                    chart.Canvas.AddDrawableTask(Stroke);
                    Stroke.ZIndex = actualZIndex + 0.2;
                    Stroke.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                    strokePathHelper.Path.ClearCommands();
                }

                foreach (var data in GetSpline(segment, secondaryScale, primaryScale, stacker))
                {
                    var s = 0f;
                    if (stacker != null)
                    {
                        s = stacker.GetStack(data.TargetPoint).Start;
                    }

                    var x = secondaryScale.ToPixels(data.TargetPoint.SecondaryValue);
                    var y = primaryScale.ToPixels(data.TargetPoint.PrimaryValue + s);

                    var visual = (LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>?)data.TargetPoint.Context.Visual;

                    if (visual == null)
                    {
                        var v = new LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>();

                        visual = v;

                        var pg = p;
                        var xg = x - hgs;
                        var yg = p - hgs;

                        var x0b = data.X0;
                        var x1b = data.X1;
                        var x2b = data.X2;
                        var y0b = p - hgs;
                        var y1b = p - hgs;
                        var y2b = p - hgs;

                        if (previousSecondaryScale != null && previousPrimaryScale != null)
                        {
                            pg = previousPrimaryScale.ToPixels(pivot);
                            xg = previousSecondaryScale.ToPixels(data.TargetPoint.SecondaryValue) - hgs;
                            yg = previousPrimaryScale.ToPixels(data.TargetPoint.PrimaryValue + s) - hgs;

                            if (data.OriginalData == null) throw new Exception("Original data not found");

                            x0b = previousSecondaryScale.ToPixels(data.OriginalData.X0);
                            x1b = previousSecondaryScale.ToPixels(data.OriginalData.X1);
                            x2b = previousSecondaryScale.ToPixels(data.OriginalData.X2);
                            y0b = chart.IsZoomingOrPanning ? previousPrimaryScale.ToPixels(data.OriginalData.Y0) : pg - hgs;
                            y1b = chart.IsZoomingOrPanning ? previousPrimaryScale.ToPixels(data.OriginalData.Y1) : pg - hgs;
                            y2b = chart.IsZoomingOrPanning ? previousPrimaryScale.ToPixels(data.OriginalData.Y2) : pg - hgs;
                        }

                        v.Geometry.X = xg;
                        v.Geometry.Y = yg;
                        v.Geometry.Width = gs;
                        v.Geometry.Height = gs;

                        v.Bezier.X0 = x0b;
                        v.Bezier.Y0 = y0b;
                        v.Bezier.X1 = x1b;
                        v.Bezier.Y1 = y1b;
                        v.Bezier.X2 = x2b;
                        v.Bezier.Y2 = y2b;

                        data.TargetPoint.Context.Visual = v;
                        OnPointCreated(data.TargetPoint);
                        v.Geometry.CompleteAllTransitions();
                        v.Bezier.CompleteAllTransitions();
                    }

                    _ = everFetched.Add(data.TargetPoint);

                    if (GeometryFill != null) GeometryFill.AddGeometyToPaintTask(visual.Geometry);
                    if (GeometryStroke != null) GeometryStroke.AddGeometyToPaintTask(visual.Geometry);

                    visual.Bezier.X0 = data.X0;
                    visual.Bezier.Y0 = data.Y0;
                    visual.Bezier.X1 = data.X1;
                    visual.Bezier.Y1 = data.Y1;
                    visual.Bezier.X2 = data.X2;
                    visual.Bezier.Y2 = data.Y2;

                    if (Fill != null)
                    {
                        if (data.IsFirst)
                        {
                            if (wasFillInitialized)
                            {
                                fillPathHelper.StartPoint.X = data.X0;
                                fillPathHelper.StartPoint.Y = p;

                                fillPathHelper.StartSegment.X = data.X0;
                                fillPathHelper.StartSegment.Y = p;

                                fillPathHelper.StartPoint.CompleteTransitions(
                                    nameof(fillPathHelper.StartPoint.Y), nameof(fillPathHelper.StartPoint.X));
                                fillPathHelper.StartSegment.CompleteTransitions(
                                    nameof(fillPathHelper.StartSegment.Y), nameof(fillPathHelper.StartSegment.X));
                            }

                            fillPathHelper.StartPoint.X = data.X0;
                            fillPathHelper.StartPoint.Y = p;
                            fillPathHelper.Path.AddCommand(fillPathHelper.StartPoint);

                            fillPathHelper.StartSegment.X = data.X0;
                            fillPathHelper.StartSegment.Y = data.Y0;
                            fillPathHelper.Path.AddCommand(fillPathHelper.StartSegment);
                        }

                        fillPathHelper.Path.AddCommand(visual.Bezier);

                        if (data.IsLast)
                        {
                            fillPathHelper.EndSegment.X = data.X2;
                            fillPathHelper.EndSegment.Y = p;
                            fillPathHelper.Path.AddCommand(fillPathHelper.EndSegment);

                            if (wasFillInitialized)
                                fillPathHelper.EndSegment.CompleteTransitions(
                                    nameof(fillPathHelper.EndSegment.Y), nameof(fillPathHelper.EndSegment.X));
                        }
                    }
                    if (Stroke != null)
                    {
                        if (data.IsFirst)
                        {
                            if (wasStrokeInitialized || chart.IsZoomingOrPanning)
                            {
                                if (chart.IsZoomingOrPanning && previousPrimaryScale != null && previousSecondaryScale != null)
                                {
                                    strokePathHelper.StartPoint.X = previousSecondaryScale.ToPixels(data.OriginalData.X0);
                                    strokePathHelper.StartPoint.Y = previousPrimaryScale.ToPixels(data.OriginalData.Y0);
                                }
                                else
                                {
                                    strokePathHelper.StartPoint.X = data.X0;
                                    strokePathHelper.StartPoint.Y = p;
                                }

                                strokePathHelper.StartPoint.CompleteTransitions(
                                   nameof(fillPathHelper.StartPoint.Y), nameof(fillPathHelper.StartPoint.X));
                            }

                            strokePathHelper.StartPoint.X = data.X0;
                            strokePathHelper.StartPoint.Y = data.Y0;
                            strokePathHelper.Path.AddCommand(strokePathHelper.StartPoint);
                        }

                        strokePathHelper.Path.AddCommand(visual.Bezier);
                    }

                    visual.Geometry.X = x - hgs;
                    visual.Geometry.Y = y - hgs;
                    visual.Geometry.Width = gs;
                    visual.Geometry.Height = gs;
                    visual.Geometry.RemoveOnCompleted = false;

                    data.TargetPoint.Context.HoverArea = new RectangleHoverArea().SetDimensions(x - hgs, y - hgs + 2 * sw, gs, gs + 2 * sw);

                    OnPointMeasured(data.TargetPoint);
                    _ = toDeletePoints.Remove(data.TargetPoint);

                    if (DataLabelsDrawableTask != null)
                    {
                        var label = (TLabel?)data.TargetPoint.Context.Label;

                        if (label == null)
                        {
                            var l = new TLabel { X = x - hgs, Y = p - hgs };

                            _ = l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                                .WithAnimation(a =>
                                    a.WithDuration(chart.AnimationsSpeed)
                                    .WithEasingFunction(chart.EasingFunction));

                            l.CompleteAllTransitions();
                            label = l;
                            data.TargetPoint.Context.Label = l;
                            DataLabelsDrawableTask.AddGeometyToPaintTask(l);
                        }

                        label.Text = DataLabelsFormatter(data.TargetPoint);
                        label.TextSize = dls;
                        label.Padding = DataLabelsPadding;
                        var labelPosition = GetLabelPosition(
                            x - hgs, y - hgs, gs, gs, label.Measure(DataLabelsDrawableTask), DataLabelsPosition,
                            SeriesProperties, data.TargetPoint.PrimaryValue > Pivot);
                        label.X = labelPosition.X;
                        label.Y = labelPosition.Y;
                    }
                }

                if (GeometryFill != null)
                {
                    chart.Canvas.AddDrawableTask(GeometryFill);
                    GeometryFill.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                    GeometryFill.ZIndex = actualZIndex + 0.3;
                }
                if (GeometryStroke != null)
                {
                    chart.Canvas.AddDrawableTask(GeometryStroke);
                    GeometryStroke.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                    GeometryStroke.ZIndex = actualZIndex + 0.4;
                }
                segmentI++;
            }

            while (segmentI > _fillPathHelperContainer.Count)
            {
                var iFill = _fillPathHelperContainer.Count - 1;
                var fillHelper = _fillPathHelperContainer[iFill];
                if (Fill != null) Fill.RemoveGeometryFromPainTask(fillHelper.Path);
                _fillPathHelperContainer.RemoveAt(iFill);

                var iStroke = _strokePathHelperContainer.Count - 1;
                var strokeHelper = _strokePathHelperContainer[iStroke];
                if (Stroke != null) Stroke.RemoveGeometryFromPainTask(strokeHelper.Path);
                _strokePathHelperContainer.RemoveAt(iStroke);
            }

            if (DataLabelsDrawableTask != null)
            {
                chart.Canvas.AddDrawableTask(DataLabelsDrawableTask);
                DataLabelsDrawableTask.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                DataLabelsDrawableTask.ZIndex = actualZIndex + 0.5;
            }

            foreach (var point in toDeletePoints)
            {
                if (point.Context.Chart != chart.View) continue;
                SoftDeletePoint(point, primaryScale, secondaryScale);
                _ = everFetched.Remove(point);
            }
        }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})"/>
        public override DimensionalBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> secondaryAxis, IAxis<TDrawingContext> primaryAxis)
        {
            var baseBounds = base.GetBounds(chart, secondaryAxis, primaryAxis);

            var tickPrimary = primaryAxis.GetTick(chart.ControlSize, baseBounds.VisiblePrimaryBounds);
            var tickSecondary = secondaryAxis.GetTick(chart.ControlSize, baseBounds.VisibleSecondaryBounds);

            var ts = tickSecondary.Value * DataPadding.X;
            var tp = tickPrimary.Value * DataPadding.Y;

            if (baseBounds.VisibleSecondaryBounds.Delta == 0)
            {
                var ms = baseBounds.VisibleSecondaryBounds.Min == 0 ? 1 : baseBounds.VisibleSecondaryBounds.Min;
                ts = 0.1 * ms * DataPadding.X;
            }

            if (baseBounds.VisiblePrimaryBounds.Delta == 0)
            {
                var mp = baseBounds.VisiblePrimaryBounds.Min == 0 ? 1 : baseBounds.VisiblePrimaryBounds.Min;
                tp = 0.1 * mp * DataPadding.Y;
            }

            return new DimensionalBounds
            {
                SecondaryBounds = new Bounds
                {
                    Max = baseBounds.SecondaryBounds.Max + ts,
                    Min = baseBounds.SecondaryBounds.Min - ts
                },
                PrimaryBounds = new Bounds
                {
                    Max = baseBounds.PrimaryBounds.Max + tp,
                    Min = baseBounds.PrimaryBounds.Min - tp
                },
                VisibleSecondaryBounds = new Bounds
                {
                    Max = baseBounds.VisibleSecondaryBounds.Max + ts,
                    Min = baseBounds.VisibleSecondaryBounds.Min - ts
                },
                VisiblePrimaryBounds = new Bounds
                {
                    Max = baseBounds.VisiblePrimaryBounds.Max + tp,
                    Min = baseBounds.VisiblePrimaryBounds.Min - tp
                },
                MinDeltaPrimary = baseBounds.MinDeltaPrimary,
                MinDeltaSecondary = baseBounds.MinDeltaSecondary
            };
        }

        /// <summary>
        /// Sets the default path transitions.
        /// </summary>
        /// <param name="areaHelper">The area helper.</param>
        /// <param name="defaultAnimation">The default animation.</param>
        /// <returns></returns>
        protected virtual void SetDefaultPathTransitions(
            AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs> areaHelper,
            Animation defaultAnimation)
        {
            _ = areaHelper.StartPoint
                .TransitionateProperties(nameof(areaHelper.StartPoint.X), nameof(areaHelper.StartPoint.Y))
                .WithAnimation(defaultAnimation)
                .CompleteCurrentTransitions();

            _ = areaHelper.StartSegment
                .TransitionateProperties(nameof(areaHelper.StartSegment.X), nameof(areaHelper.StartSegment.Y))
                .WithAnimation(defaultAnimation)
                .CompleteCurrentTransitions();

            _ = areaHelper.EndSegment
                .TransitionateProperties(nameof(areaHelper.EndSegment.X), nameof(areaHelper.EndSegment.Y))
                .WithAnimation(defaultAnimation)
                .CompleteCurrentTransitions();
        }

        /// <inheritdoc cref="DrawableSeries{TModel, TVisual, TLabel, TDrawingContext}.OnPaintContextChanged"/>
        protected override void OnPaintContextChanged()
        {
            var context = new PaintContext<TDrawingContext>();
            var lss = (float)LegendShapeSize;
            var w = LegendShapeSize;
            var sh = 0f;

            if (_geometryStroke != null)
            {
                var strokeClone = _geometryStroke.CloneTask();
                var visual = new TVisual
                {
                    X = _geometryStroke.StrokeThickness,
                    Y = _geometryStroke.StrokeThickness,
                    Height = lss,
                    Width = lss
                };
                sh = _geometryStroke.StrokeThickness;
                strokeClone.ZIndex = 1;
                w += 2 * _geometryStroke.StrokeThickness;
                strokeClone.AddGeometyToPaintTask(visual);
                _ = context.PaintTasks.Add(strokeClone);
            }
            else if (Stroke != null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = strokeClone.StrokeThickness,
                    Y = strokeClone.StrokeThickness,
                    Height = lss,
                    Width = lss
                };
                sh = strokeClone.StrokeThickness;
                strokeClone.ZIndex = 1;
                w += 2 * strokeClone.StrokeThickness;
                strokeClone.AddGeometyToPaintTask(visual);
                _ = context.PaintTasks.Add(strokeClone);
            }

            if (_geometryFill != null)
            {
                var fillClone = _geometryFill.CloneTask();
                var visual = new TVisual { X = sh, Y = sh, Height = lss, Width = lss };
                fillClone.AddGeometyToPaintTask(visual);
                _ = context.PaintTasks.Add(fillClone);
            }
            else if (Fill != null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual { X = sh, Y = sh, Height = lss, Width = lss };
                fillClone.AddGeometyToPaintTask(visual);
                _ = context.PaintTasks.Add(fillClone);
            }

            context.Width = w;
            context.Height = w;

            paintContext = context;
            OnPropertyChanged(nameof(DefaultPaintContext));
        }

        private IEnumerable<BezierData> GetSpline(
            ChartPoint[] points,
            Scaler xScale,
            Scaler yScale,
            StackPosition<TDrawingContext>? stacker)
        {
            if (points.Length == 0) yield break;

            ChartPoint previous, current, next, next2;

            for (var i = 0; i < points.Length; i++)
            {
                previous = points[i - 1 < 0 ? 0 : i - 1];
                current = points[i];
                next = points[i + 1 > points.Length - 1 ? points.Length - 1 : i + 1];
                next2 = points[i + 2 > points.Length - 1 ? points.Length - 1 : i + 2];

                var pys = 0f;
                var cys = 0f;
                var nys = 0f;
                var nnys = 0f;

                if (stacker != null)
                {
                    pys = stacker.GetStack(previous).Start;
                    cys = stacker.GetStack(current).Start;
                    nys = stacker.GetStack(next).Start;
                    nnys = stacker.GetStack(next2).Start;
                }

                var xc1 = (previous.SecondaryValue + current.SecondaryValue) / 2.0f;
                var yc1 = (previous.PrimaryValue + pys + current.PrimaryValue + cys) / 2.0f;
                var xc2 = (current.SecondaryValue + next.SecondaryValue) / 2.0f;
                var yc2 = (current.PrimaryValue + cys + next.PrimaryValue + nys) / 2.0f;
                var xc3 = (next.SecondaryValue + next2.SecondaryValue) / 2.0f;
                var yc3 = (next.PrimaryValue + nys + next2.PrimaryValue + nnys) / 2.0f;

                var len1 = (float)Math.Sqrt(
                    (current.SecondaryValue - previous.SecondaryValue) *
                    (current.SecondaryValue - previous.SecondaryValue) +
                    (current.PrimaryValue + cys - previous.PrimaryValue + pys) * (current.PrimaryValue + cys - previous.PrimaryValue + pys));
                var len2 = (float)Math.Sqrt(
                    (next.SecondaryValue - current.SecondaryValue) *
                    (next.SecondaryValue - current.SecondaryValue) +
                    (next.PrimaryValue + nys - current.PrimaryValue + cys) * (next.PrimaryValue + nys - current.PrimaryValue + cys));
                var len3 = (float)Math.Sqrt(
                    (next2.SecondaryValue - next.SecondaryValue) *
                    (next2.SecondaryValue - next.SecondaryValue) +
                    (next2.PrimaryValue + nnys - next.PrimaryValue + nys) * (next2.PrimaryValue + nnys - next.PrimaryValue + nys));

                var k1 = len1 / (len1 + len2);
                var k2 = len2 / (len2 + len3);

                if (float.IsNaN(k1)) k1 = 0f;
                if (float.IsNaN(k2)) k2 = 0f;

                var xm1 = xc1 + (xc2 - xc1) * k1;
                var ym1 = yc1 + (yc2 - yc1) * k1;
                var xm2 = xc2 + (xc3 - xc2) * k2;
                var ym2 = yc2 + (yc3 - yc2) * k2;

                var c1X = xm1 + (xc2 - xm1) * _lineSmoothness + current.SecondaryValue - xm1;
                var c1Y = ym1 + (yc2 - ym1) * _lineSmoothness + current.PrimaryValue + cys - ym1;
                var c2X = xm2 + (xc2 - xm2) * _lineSmoothness + next.SecondaryValue - xm2;
                var c2Y = ym2 + (yc2 - ym2) * _lineSmoothness + next.PrimaryValue + nys - ym2;

                float x0, y0;

                if (i == 0)
                {
                    x0 = current.SecondaryValue;
                    y0 = current.PrimaryValue + cys;
                }
                else
                {
                    x0 = c1X;
                    y0 = c1Y;
                }

                yield return new BezierData(points[i])
                {
                    IsFirst = i == 0,
                    IsLast = i == points.Length - 1,
                    X0 = xScale.ToPixels(x0),
                    Y0 = yScale.ToPixels(y0),
                    X1 = xScale.ToPixels(c2X),
                    Y1 = yScale.ToPixels(c2Y),
                    X2 = xScale.ToPixels(next.SecondaryValue),
                    Y2 = yScale.ToPixels(next.PrimaryValue + nys),
                    OriginalData = new BezierData(points[i])
                    {
                        X0 = x0,
                        Y0 = y0,
                        X1 = c2X,
                        Y1 = c2Y,
                        X2 = next.SecondaryValue,
                        Y2 = next.PrimaryValue + nys,
                    }
                };
            }
        }

        private IEnumerable<ChartPoint[]> SplitEachNull(
            ChartPoint[] points,
            Scaler xScale,
            Scaler yScale)
        {
            var l = new List<ChartPoint>(points.Length);

            foreach (var point in points)
            {
                if (point.IsNull)
                {
                    if (point.Context.Visual is LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs> visual)
                    {
                        var x = xScale.ToPixels(point.SecondaryValue);
                        var y = yScale.ToPixels(point.PrimaryValue);
                        var gs = _geometrySize;
                        var hgs = gs / 2f;
                        var sw = Stroke?.StrokeThickness ?? 0;
                        var p = yScale.ToPixels(pivot);
                        visual.Geometry.X = x - hgs;
                        visual.Geometry.Y = p - hgs;
                        visual.Geometry.Width = gs;
                        visual.Geometry.Height = gs;
                        visual.Geometry.RemoveOnCompleted = true;
                        point.Context.Visual = null;
                    }

                    if (l.Count > 0) yield return l.ToArray();
                    l = new List<ChartPoint>(points.Length);
                    continue;
                }

                l.Add(point);
            }

            if (l.Count > 0) yield return l.ToArray();
        }

        /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SetDefaultPointTransitions(ChartPoint)"/>
        protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
        {
            var chart = chartPoint.Context.Chart;

            if (chartPoint.Context.Visual is not LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs> visual) throw new Exception("Unable to initialize the point instance.");

            _ = visual.Geometry
                .TransitionateProperties(
                    nameof(visual.Geometry.X),
                    nameof(visual.Geometry.Y),
                    nameof(visual.Geometry.Width),
                    nameof(visual.Geometry.Height))
                .WithAnimation(animation =>
                    animation
                        .WithDuration(chart.AnimationsSpeed)
                        .WithEasingFunction(chart.EasingFunction));

            _ = visual.Bezier
                .TransitionateProperties(
                    nameof(visual.Bezier.X0),
                    nameof(visual.Bezier.Y0),
                    nameof(visual.Bezier.X1),
                    nameof(visual.Bezier.Y1),
                    nameof(visual.Bezier.X2),
                    nameof(visual.Bezier.Y2))
                .WithAnimation(animation =>
                    animation
                         .WithDuration(chart.AnimationsSpeed)
                        .WithEasingFunction(chart.EasingFunction));
        }

        /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SoftDeletePoint(ChartPoint, Scaler, Scaler)"/>
        protected override void SoftDeletePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
        {
            var visual = (LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>?)point.Context.Visual;
            if (visual == null) return;

            var chartView = (ICartesianChartView<TDrawingContext>)point.Context.Chart;
            if (chartView.Core.IsZoomingOrPanning)
            {
                visual.Geometry.CompleteAllTransitions();
                visual.Geometry.RemoveOnCompleted = true;
                return;
            }

            var gs = _geometrySize;
            var hgs = gs / 2f;

            visual.Geometry.X = secondaryScale.ToPixels(point.SecondaryValue) - hgs;
            visual.Geometry.Y = primaryScale.ToPixels(point.PrimaryValue) - hgs;
            visual.Geometry.Height = 0;
            visual.Geometry.Width = 0;
            visual.Geometry.RemoveOnCompleted = true;
        }

        /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.Delete(IChartView)"/>
        public override void Delete(IChartView chart)
        {
            base.Delete(chart);

            if (Fill != null)
                foreach (var pathHelper in _fillPathHelperContainer.ToArray())
                    Fill.RemoveGeometryFromPainTask(pathHelper.Path);

            if (Stroke != null)
                foreach (var pathHelper in _strokePathHelperContainer.ToArray())
                    Stroke.RemoveGeometryFromPainTask(pathHelper.Path);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <returns></returns>
        public override void Dispose()
        {
            foreach (var chart in subscribedTo)
            {
                var c = (Chart<TDrawingContext>)chart;
                if (_geometryFill != null) c.Canvas.RemovePaintTask(_geometryFill);
                if (_geometryStroke != null) c.Canvas.RemovePaintTask(_geometryStroke);
            }

            base.Dispose();
        }
    }
}

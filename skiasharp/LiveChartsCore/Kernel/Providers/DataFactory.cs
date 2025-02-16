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

using System;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;

namespace LiveChartsCore.Kernel.Providers;

/// <summary>
/// Defines the default data factory.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TDrawingContext"></typeparam>
public class DataFactory<TModel, TDrawingContext>
    where TDrawingContext : DrawingContext
{
    private readonly bool _isTModelChartEntity = false;
    private readonly Dictionary<object, Dictionary<int, MappedChartEntity>> _chartIndexEntityMap = new();
    private ISeries? _series;

    /// <summary>
    /// Gets or sets the previous known bounds.
    /// </summary>
    protected DimensionalBounds PreviousKnownBounds { get; set; } = new DimensionalBounds(true);

    /// <summary>
    /// Initializes a new instance of the <see cref="DataFactory{TModel, TDrawingContext}"/> class.
    /// </summary>
    public DataFactory()
    {
        var bounds = new DimensionalBounds(true);
        PreviousKnownBounds = bounds;
        _isTModelChartEntity = typeof(IChartEntity).IsAssignableFrom(typeof(TModel));
    }

    /// <summary>
    /// Fetches the points for the specified series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <param name="chart">The chart.</param>
    /// <returns></returns>
    public virtual IEnumerable<ChartPoint> Fetch(ISeries<TModel> series, IChart chart)
    {
        if (series.Values is null) yield break;
        _series = series;

        foreach (var value in GetEntities(series, chart))
        {
            if (value is null)
            {
                yield return ChartPoint.Empty;
                continue;
            }

            yield return value.MetaData?.ChartPoints?.GetPointForView(chart.View) ?? ChartPoint.Empty;
        }
    }

    /// <summary>
    /// Disposes a given point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns></returns>
    public virtual void DisposePoint(ChartPoint point)
    {
        if (_isTModelChartEntity) return;

        var canvas = (MotionCanvas<TDrawingContext>)point.Context.Chart.CoreChart.Canvas;
        _ = _chartIndexEntityMap.TryGetValue(canvas.Sync, out var d);
        var map = d;
        if (map is null) return;
        _ = map.Remove(point.Context.Entity.MetaData?.EntityIndex ?? 0);
    }

    /// <summary>
    /// Disposes the data provider from the given chart.
    /// </summary>
    /// <param name="chart"></param>
    public virtual void Dispose(IChart chart)
    {
        _series = null;
        if (_isTModelChartEntity) return;

        var canvas = (MotionCanvas<TDrawingContext>)chart.Canvas;
        _ = _chartIndexEntityMap.Remove(canvas.Sync);
    }

    /// <summary>
    /// Gets the Cartesian bounds.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="series">The series.</param>
    /// <param name="plane1">The x.</param>
    /// <param name="plane2">The y.</param>
    /// <returns></returns>
    public virtual SeriesBounds GetCartesianBounds(
        Chart<TDrawingContext> chart,
        IChartSeries<TDrawingContext> series,
        IPlane plane1,
        IPlane plane2)
    {
        var stack = chart.SeriesContext.GetStackPosition(series, series.GetStackGroup());

        var xMin = plane1.MinLimit ?? double.MinValue;
        var xMax = plane1.MaxLimit ?? double.MaxValue;
        var yMin = plane2.MinLimit ?? double.MinValue;
        var yMax = plane2.MaxLimit ?? double.MaxValue;

        var hasData = false;

        var bounds = new DimensionalBounds();

        ChartPoint? previous = null;

        foreach (var point in series.Fetch(chart))
        {
            if (point.IsEmpty) continue;

            var coordinate = point.Coordinate;

            var primary = coordinate.PrimaryValue;
            var secondary = coordinate.SecondaryValue;
            var tertiary = coordinate.TertiaryValue;

            if (stack is not null) primary = stack.StackPoint(point);

            bounds.PrimaryBounds.AppendValue(primary);
            bounds.SecondaryBounds.AppendValue(secondary);
            bounds.TertiaryBounds.AppendValue(tertiary);

            if (!coordinate.PointError.IsEmpty)
            {
                var e = coordinate.PointError;
                bounds.PrimaryBounds.AppendValue(primary + e.Yi);
                bounds.PrimaryBounds.AppendValue(primary - e.Yj);
                bounds.SecondaryBounds.AppendValue(secondary - e.Xi);
                bounds.SecondaryBounds.AppendValue(secondary + e.Xj);
            }

            if (primary >= yMin && primary <= yMax && secondary >= xMin && secondary <= xMax)
            {
                bounds.VisiblePrimaryBounds.AppendValue(primary);
                bounds.VisibleSecondaryBounds.AppendValue(secondary);
                bounds.VisibleTertiaryBounds.AppendValue(tertiary);

                if (!coordinate.PointError.IsEmpty)
                {
                    var e = coordinate.PointError;
                    bounds.VisiblePrimaryBounds.AppendValue(primary + e.Yi);
                    bounds.VisiblePrimaryBounds.AppendValue(primary - e.Yj);
                    bounds.VisibleSecondaryBounds.AppendValue(secondary - e.Xi);
                    bounds.VisibleSecondaryBounds.AppendValue(secondary + e.Xj);
                }
            }

            if (previous is not null)
            {
                var previousCoordinate = previous.Coordinate;

                var dx = Math.Abs(previousCoordinate.SecondaryValue - coordinate.SecondaryValue);
                var dy = Math.Abs(previousCoordinate.PrimaryValue - coordinate.PrimaryValue);
                if (dx < bounds.SecondaryBounds.MinDelta) bounds.SecondaryBounds.MinDelta = dx;
                if (dy < bounds.PrimaryBounds.MinDelta) bounds.PrimaryBounds.MinDelta = dy;
            }

            previous = point;
            hasData = true;
        }

        return !hasData
            ? new SeriesBounds(PreviousKnownBounds, true)
            : new SeriesBounds(PreviousKnownBounds = bounds, false);
    }

    /// <summary>
    /// Gets the financial bounds.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="series">The series.</param>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns></returns>
    public virtual SeriesBounds GetFinancialBounds(
        CartesianChart<TDrawingContext> chart,
        IChartSeries<TDrawingContext> series,
        ICartesianAxis x,
        ICartesianAxis y)
    {
        var xMin = x.MinLimit ?? double.MinValue;
        var xMax = x.MaxLimit ?? double.MaxValue;
        var yMin = y.MinLimit ?? double.MinValue;
        var yMax = y.MaxLimit ?? double.MaxValue;

        var hasData = false;

        var bounds = new DimensionalBounds();
        ChartPoint? previous = null;
        foreach (var point in series.Fetch(chart))
        {
            if (point.IsEmpty) continue;

            var coordinate = point.Coordinate;

            var primaryMax = coordinate.PrimaryValue;
            var primaryMin = coordinate.QuinaryValue;
            var secondary = coordinate.SecondaryValue;
            var tertiary = coordinate.TertiaryValue;

            bounds.PrimaryBounds.AppendValue(primaryMax);
            bounds.PrimaryBounds.AppendValue(primaryMin);
            bounds.SecondaryBounds.AppendValue(secondary);
            bounds.TertiaryBounds.AppendValue(tertiary);

            if (primaryMax >= yMin && primaryMin <= yMax && secondary >= xMin && secondary <= xMax)
            {
                bounds.VisiblePrimaryBounds.AppendValue(primaryMax);
                bounds.VisiblePrimaryBounds.AppendValue(primaryMin);
                bounds.VisibleSecondaryBounds.AppendValue(secondary);
                bounds.VisibleTertiaryBounds.AppendValue(tertiary);
            }

            if (previous is not null)
            {
                var previousCoordinate = previous.Coordinate;

                var dx = Math.Abs(previousCoordinate.SecondaryValue - coordinate.SecondaryValue);
                var dy = Math.Abs(previousCoordinate.PrimaryValue - coordinate.PrimaryValue);
                if (dx < bounds.SecondaryBounds.MinDelta) bounds.SecondaryBounds.MinDelta = dx;
                if (dy < bounds.PrimaryBounds.MinDelta) bounds.PrimaryBounds.MinDelta = dy;
            }

            previous = point;
            hasData = true;
        }

        return !hasData
            ? new SeriesBounds(PreviousKnownBounds, true)
            : new SeriesBounds(PreviousKnownBounds = bounds, false);
    }

    /// <summary>
    /// Gets the pie bounds.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="series">The series.</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException">Unexpected null stacker</exception>
    public virtual SeriesBounds GetPieBounds(
        PieChart<TDrawingContext> chart, IPieSeries<TDrawingContext> series)
    {
        var stack =
            chart.SeriesContext.GetStackPosition(series, series.GetStackGroup())
            ?? throw new NullReferenceException("Unexpected null stacker");
        var bounds = new DimensionalBounds();
        var hasData = false;

        foreach (var point in series.Fetch(chart))
        {
            if (point.IsEmpty) continue;

            _ = stack.StackPoint(point);
            bounds.PrimaryBounds.AppendValue(point.Coordinate.PrimaryValue);
            bounds.SecondaryBounds.AppendValue(point.Coordinate.SecondaryValue);
            bounds.TertiaryBounds.AppendValue(series.Pushout > series.HoverPushout ? series.Pushout : series.HoverPushout);
            hasData = true;
        }

        if (!hasData)
        {
            bounds.PrimaryBounds.AppendValue(0);
            bounds.SecondaryBounds.AppendValue(0);
            bounds.TertiaryBounds.AppendValue(0);
        }

        return new SeriesBounds(bounds, false);
    }


    /// <summary>
    /// Clears the visuals in the cache.
    /// </summary>
    public void RestartVisuals()
    {
        if (_series is not null && _series.Values is IEnumerable<IChartEntity> entities)
        {
            foreach (var entity in entities)
            {
                foreach (var chartPoint in entity.MetaData?.ChartPoints?.Values ?? Enumerable.Empty<ChartPoint>())
                {
                    if (chartPoint.Context.Visual is not IAnimatable visual) continue;
                    visual.RemoveTransition(null);
                }
            }
        }

        foreach (var item in _chartIndexEntityMap.Values)
        {
            foreach (var index in item.Values)
            {
                foreach (var chartPoint in index.MetaData?.ChartPoints?.Values ?? Enumerable.Empty<ChartPoint>())
                {
                    if (chartPoint.Context.Visual is not IAnimatable visual) continue;
                    visual.RemoveTransition(null);
                }
            }
        }
        _chartIndexEntityMap.Clear();
    }

    private IEnumerable<IChartEntity?> GetEntities(ISeries<TModel> series, IChart chart)
    {
        return _isTModelChartEntity
            ? EnumerateChartEntities(series, chart)
            : EnumerateIndexedEntities(series, chart);
    }

    private IEnumerable<IChartEntity> EnumerateChartEntities(ISeries<TModel> series, IChart chart)
    {
        if (series.Values is null) yield break;
        var entities = (IEnumerable<IChartEntity>)series.Values;
        var index = 0;

        foreach (var entity in entities)
        {
            if (entity is null)
            {
                index++;
                yield return new MappedChartEntity();
                continue;
            }

            entity.MetaData ??= new ChartEntityMetaData();

            if (!entity.MetaData.ChartPoints.TryGetValue(chart.View, out var point))
            {
                point = new ChartPoint(chart.View, series, entity);
                entity.MetaData.ChartPoints[chart.View] = point;
                point.Context.DataSource = entity;
            }

            entity.MetaData.EntityIndex = index++;

            yield return entity;
        }
    }

    private IEnumerable<IChartEntity?> EnumerateIndexedEntities(ISeries<TModel> series, IChart chart)
    {
        if (series.Values is null) yield break;

        var canvas = (MotionCanvas<TDrawingContext>)chart.Canvas;
        var mapper = series.Mapping ?? LiveCharts.DefaultSettings.GetMap<TModel>();
        var index = 0;

        _ = _chartIndexEntityMap.TryGetValue(canvas.Sync, out var d);
        if (d is null)
        {
            d = new Dictionary<int, MappedChartEntity>();
            _chartIndexEntityMap[canvas.Sync] = d;
        }
        var IndexEntityMap = d;

        foreach (var item in series.Values)
        {
            if (item is null)
            {
                yield return new MappedChartEntity();
                index++;
                continue;
            }

            if (!IndexEntityMap.TryGetValue(index, out var entity))
                IndexEntityMap[index] = entity = new MappedChartEntity { MetaData = new ChartEntityMetaData() };

            if (!entity.MetaData!.ChartPoints!.TryGetValue(chart.View, out var point))
            {
                point = new ChartPoint(chart.View, series, entity);
                entity.MetaData.ChartPoints[chart.View] = point;
            }

            point.Context.DataSource = item;
            point.Coordinate = mapper(item, index);
            entity.MetaData.EntityIndex = index++;

            yield return entity;
        }
    }
}

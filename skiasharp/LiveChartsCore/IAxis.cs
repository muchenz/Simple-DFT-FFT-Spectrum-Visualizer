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
using System;
using System.Drawing;
using LiveChartsCore.Measure;
using System.Collections.Generic;
using LiveChartsCore.Drawing.Common;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines an Axis in a Cartesian chart.
    /// </summary>
    public interface IAxis: IDisposable
    {
        /// <summary>
        /// Gets the previous data bounds.
        /// </summary>
        /// <value>
        /// The previous data bounds.
        /// </value>
        Bounds? PreviousDataBounds { get; }

        /// <summary>
        /// Gets the data bounds, the min and max values in the axis.
        /// </summary>
        /// <value>
        /// The data bounds.
        /// </value>
        Bounds DataBounds { get; }

        /// <summary>
        /// Gets the data visible bounds, the min and max visible values in the axis.
        /// </summary>
        /// <value>
        /// The data bounds.
        /// </value>
        Bounds VisibleDataBounds { get; }

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        AxisOrientation Orientation { get; }

        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        /// <value>
        /// The padding.
        /// </value>
        Padding Padding { get; set; }

        /// <summary>
        /// Gets or sets the xo, a refence used internally to calculate the axis position.
        /// </summary>
        /// <value>
        /// The xo.
        /// </value>
        float Xo { get; set; }

        /// <summary>
        /// Gets or sets the yo, a refence used internally to calculate the axis position..
        /// </summary>
        /// <value>
        /// The yo.
        /// </value>
        float Yo { get; set; }

        /// <summary>
        /// Gets or sets the labeler, a function that receives a number and return the label content as string.
        /// </summary>
        /// <value>
        /// The labeler.
        /// </value>
        Func<double, string> Labeler { get; set; }

        /// <summary>
        /// Gets or sets the minimum step, the step defines the interval between every separator in the axis,
        /// LiveCharts will calculate it automatically based on the chart data and the chart size size, if the calculated step is less than the <see cref="MinStep"/> 
        /// then <see cref="MinStep"/> will be used as the axis step, default is 0.
        /// </summary>
        /// <value>
        /// The step.
        /// </value>
        double MinStep { get; set; }

        /// <summary>
        /// Gets or sets the unit with, it means the width of every point (if the series requires it) in the chart values scale, this value
        /// should normally be 1, unless you are plotting in a custom scale, default is 1.
        /// </summary>
        /// <value>
        /// The unit with.
        /// </value>
        double UnitWith { get; set; }

        /// <summary>
        /// Gets or sets the minimum value visible in the axis, any point less than this value will be hidden, 
        /// set it to null to use a value based on the smaller value in the chart.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        double? MinLimit { get; set; }

        /// <summary>
        /// Gets or sets the maximum value visible in the axis, any point greater than this value will be hidden, 
        /// set it null to use a value based on the greather value in the chart.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        double? MaxLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the axis is inverted based on the Cartesian coordinate system.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is inverted; otherwise, <c>false</c>.
        /// </value>
        bool IsInverted { get; set; }

        /// <summary>
        /// Gets or sets the axis position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        AxisPosition Position { get; set; }

        /// <summary>
        /// Gets or sets the labels rotation.
        /// </summary>
        /// <value>
        /// The labels rotation.
        /// </value>
        double LabelsRotation { get; set; }

        /// <summary>
        /// Gets or sets the size of the text.
        /// </summary>
        /// <value>
        /// The size of the text.
        /// </value>
        double TextSize { get; set; }

        /// <summary>
        /// Gets or sets the labels, if labels are not null, then the axis label will be pulled from the labels collection,
        /// the label is mapped to the chart based on the position of the label and the position of the point, both integers,
        /// if the axis requires a label outsite the bounds of the labels IList then the index will be returned as the label.
        /// Default value is null.
        /// </summary>
        /// <value>
        /// The labels.
        /// </value>
        IList<string>? Labels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the separator lines are visible.
        /// </summary>
        bool ShowSeparatorLines { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the separator weddges are visible.
        /// </summary>
        bool ShowSeparatorWedges { get; set; }

        /// <summary>
        /// Initializes the axis for the specified orientation.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        void Initialize(AxisOrientation orientation);
    }

    public interface IAxis<TDrawingContext> : IAxis
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets or sets the text brush.
        /// </summary>
        /// <value>
        /// The text brush.
        /// </value>
        IDrawableTask<TDrawingContext>? TextBrush { get; set; }

        /// <summary>
        /// Gets or sets the separators brush.
        /// </summary>
        /// <value>
        /// The separators brush.
        /// </value>
        IDrawableTask<TDrawingContext>? SeparatorsBrush { get; set; }

        /// <summary>
        /// Measures the axis.
        /// </summary>
        /// <param name="chart">The chart.</param>
        void Measure(CartesianChart<TDrawingContext> chart);

        /// <summary>
        /// Gets the size of the possible.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <returns></returns>
        SizeF GetPossibleSize(CartesianChart<TDrawingContext> chart);

        /// <summary>
        /// Gets the deleting tasks.
        /// </summary>
        /// <value>
        /// The deleting tasks.
        /// </value>
        List<IDrawableTask<TDrawingContext>> DeletingTasks { get; }
    }
}
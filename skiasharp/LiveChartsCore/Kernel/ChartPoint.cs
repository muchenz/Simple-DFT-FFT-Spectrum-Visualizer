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

namespace LiveChartsCore.Kernel
{
    /// <summary>
    /// Defiens a point in a chart.
    /// </summary>
    public class ChartPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPoint"/> class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="series">The series.</param>
        public ChartPoint(IChartView chart, ISeries series)
        {
            Context = new ChartPointContext(chart, series);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is null.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is null; otherwise, <c>false</c>.
        /// </value>
        public bool IsNull { get; set; }

        /// <summary>
        /// Gets or sets the primary value.
        /// </summary>
        /// <value>
        /// The primary value.
        /// </value>
        public float PrimaryValue { get; set; }

        /// <summary>
        /// Gets or sets the secondary value.
        /// </summary>
        /// <value>
        /// The secondary value.
        /// </value>
        public float SecondaryValue { get; set; }

        /// <summary>
        /// Gets or sets the tertiary value.
        /// </summary>
        /// <value>
        /// The tertiary value.
        /// </value>
        public float TertiaryValue { get; set; }

        /// <summary>
        /// Gets the point as tooltip string.
        /// </summary>
        /// <value>
        /// As tooltip string.
        /// </value>
        public string AsTooltipString => Context.Series.TooltipLabelFormatter(this);

        /// <summary>
        /// Gets the point as data label.
        /// </summary>
        /// <value>
        /// As tooltip string.
        /// </value>
        public string AsDataLabel => Context.Series.DataLabelsFormatter(this);

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public ChartPointContext Context { get; }
    }
}
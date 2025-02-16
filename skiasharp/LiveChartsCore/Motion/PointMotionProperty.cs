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

using LiveChartsCore.Drawing;

namespace LiveChartsCore.Motion;

/// <summary>
/// Defines the <see cref="LvcPoint"/> motion property class.
/// </summary>
public class PointMotionProperty : MotionProperty<LvcPoint>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PointMotionProperty"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public PointMotionProperty(string propertyName)
        : base(propertyName)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PointMotionProperty"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    public PointMotionProperty(string propertyName, LvcPoint value)
        : base(propertyName)
    {
        fromValue = value;
        toValue = value;
    }

    /// <inheritdoc cref="MotionProperty{T}.OnGetMovement(float)" />
    protected override LvcPoint OnGetMovement(float progress)
    {
        return new LvcPoint(
            fromValue.X + progress * (toValue.X - fromValue.X),
            fromValue.Y + progress * (toValue.Y - fromValue.Y));
    }
}

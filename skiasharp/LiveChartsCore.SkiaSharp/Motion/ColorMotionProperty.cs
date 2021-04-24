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

using LiveChartsCore.Motion;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Motion
{
    public class ColorMotionProperty : MotionProperty<SKColor>
    {
        public ColorMotionProperty(string propertyName)
            : base(propertyName)
        {
            fromValue = new SKColor();
            toValue = new SKColor();
        }

        public ColorMotionProperty(string propertyName, SKColor color)
            : base(propertyName)
        {
            fromValue = new SKColor(color.Red, color.Green, color.Blue, color.Alpha);
            toValue = new SKColor(color.Red, color.Green, color.Blue, color.Alpha);
        }

        protected override SKColor OnGetMovement(float progress)
        {
            unchecked
            {
                return new SKColor(
                    (byte)(fromValue.Red + progress * (toValue.Red - fromValue.Red)),
                    (byte)(fromValue.Green + progress * (toValue.Green - fromValue.Green)),
                    (byte)(fromValue.Blue + progress * (toValue.Blue - fromValue.Blue)),
                    (byte)(fromValue.Alpha + progress * (toValue.Alpha - fromValue.Alpha)));
            }
        }
    }
}

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
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.Motion;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries.Segments
{
    public class CubicBezierSegment : PathCommand, IBezierSegment<SKPath>
    {
        private readonly FloatMotionProperty x0Transition;
        private readonly FloatMotionProperty y0Transition;
        private readonly FloatMotionProperty x1Transition;
        private readonly FloatMotionProperty y1Transition;
        private readonly FloatMotionProperty x2Transition;
        private readonly FloatMotionProperty y2Transition;

        public CubicBezierSegment()
        {
            x0Transition = RegisterMotionProperty(new FloatMotionProperty(nameof(X0), 0f));
            y0Transition = RegisterMotionProperty(new FloatMotionProperty(nameof(Y0), 0f));
            x1Transition = RegisterMotionProperty(new FloatMotionProperty(nameof(X1), 0f));
            y1Transition = RegisterMotionProperty(new FloatMotionProperty(nameof(Y1), 0f));
            x2Transition = RegisterMotionProperty(new FloatMotionProperty(nameof(X2), 0f));
            y2Transition = RegisterMotionProperty(new FloatMotionProperty(nameof(Y2), 0f));
        }

        public float X0 { get => x0Transition.GetMovement(this); set => x0Transition.SetMovement(value, this); }

        public float Y0 { get => y0Transition.GetMovement(this); set => y0Transition.SetMovement(value, this); }

        public float X1 { get => x1Transition.GetMovement(this); set => x1Transition.SetMovement(value, this); }

        public float Y1 { get => y1Transition.GetMovement(this); set => y1Transition.SetMovement(value, this); }

        public float X2 { get => x2Transition.GetMovement(this); set => x2Transition.SetMovement(value, this); }

        public float Y2 { get => y2Transition.GetMovement(this); set => y2Transition.SetMovement(value, this); }

        public override void Execute(SKPath path, long currentTime, Animatable pathGeometry)
        {
            SetCurrentTime(currentTime);
            path.CubicTo(X0, Y0, X1, Y1, X2, Y2);
        }
    }
}

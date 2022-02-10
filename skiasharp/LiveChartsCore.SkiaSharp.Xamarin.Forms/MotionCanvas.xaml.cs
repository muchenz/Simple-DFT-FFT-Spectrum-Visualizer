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
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms
{
    /// <summary>
    /// The motion canvas control fro Xamarin, <see cref="MotionCanvas{TDrawingContext}"/>.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MotionCanvas : ContentView
    {
        private bool isDrawingLoopRunning = false;
        private double framesPerSecond = 90;

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
        /// </summary>
        public MotionCanvas()
        {
            InitializeComponent();
            if (skiaElement == null)
                throw new Exception(
                    $"SkiaElement not found. This was probably caused because the control {nameof(MotionCanvas)} template was overridden, " +
                    $"If you override the template please add an {nameof(SKCanvasView)} to the template and name it 'skiaElement'");

            skiaElement.PaintSurface += OnCanvasViewPaintSurface;
            CanvasCore.Invalidated += OnCanvasCoreInvalidated;
        }

        /// <summary>
        /// The paint tasks property
        /// </summary>
        public static readonly BindableProperty PaintTasksProperty = BindableProperty.Create(
            nameof(PaintTasks), typeof(HashSet<IDrawableTask<SkiaSharpDrawingContext>>),
            typeof(MotionCanvas), propertyChanged: PaintTasksChanged);

        /// <summary>
        /// Gets the sk canvas view.
        /// </summary>
        /// <value>
        /// The sk canvas view.
        /// </value>
        public SKCanvasView SkCanvasView => skiaElement;

        /// <summary>
        /// Gets or sets the frames per second.
        /// </summary>
        /// <value>
        /// The frames per second.
        /// </value>
        public double FramesPerSecond { get => framesPerSecond; set => framesPerSecond = value; }

        /// <summary>
        /// Gets or sets the paint tasks.
        /// </summary>
        /// <value>
        /// The paint tasks.
        /// </value>
        public HashSet<IDrawableTask<SkiaSharpDrawingContext>> PaintTasks
        {
            get => (HashSet<IDrawableTask<SkiaSharpDrawingContext>>)GetValue(PaintTasksProperty);
            set => SetValue(PaintTasksProperty, value);
        }

        /// <summary>
        /// Gets the canvas core.
        /// </summary>
        /// <value>
        /// The canvas core.
        /// </value>
        public MotionCanvas<SkiaSharpDrawingContext> CanvasCore { get; } = new();

        /// <summary>
        /// Invalidates this instance.
        /// </summary>
        /// <returns></returns>
        public void Invalidate()
        {
            MainThread.BeginInvokeOnMainThread(RunDrawingLoop);
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            CanvasCore.DrawFrame(new SkiaSharpDrawingContext(args.Info, args.Surface, args.Surface.Canvas));
        }

        private void OnCanvasCoreInvalidated(MotionCanvas<SkiaSharpDrawingContext> sender)
        {
            Invalidate();
        }

        private async void RunDrawingLoop()
        {
            if (isDrawingLoopRunning) return;
            isDrawingLoopRunning = true;

            var ts = TimeSpan.FromSeconds(1 / framesPerSecond);
            while (!CanvasCore.IsValid)
            {
                skiaElement.InvalidateSurface();
                await Task.Delay(ts);
            }

            isDrawingLoopRunning = false;
        }

        private static void PaintTasksChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (MotionCanvas)bindable;
            control.CanvasCore.SetPaintTasks(control.PaintTasks);
        }
    }
}
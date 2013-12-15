using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICFP2009.VirtualMachineLib;
using Vector=ICFP2009.Common.Vector;

namespace ICFP2009.Visualizer.Controls
{
    public class SpaceCanvas : Control
    {
        private int _currentProblem;
        private Point _maxDistancePoint = new Point(0, 0);

        static SpaceCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof (SpaceCanvas), new FrameworkPropertyMetadata(typeof (SpaceCanvas)));
        }

        public SpaceCanvas()
        {
            VirtualMachine.Instance.StateReseted += VMStateReseted;
            VirtualMachine.Instance.StepCompleted += VMStepCompleted;
        }

        public double AdditionalFactor { get; set; }

        private void VMStepCompleted(object sender, StepCompletedEventArgs e)
        {
            StepCompleted();
        }

        private void VMStateReseted(object sender, StateResetedEventArgs e)
        {
            Reset();
        }

        private void StepCompleted()
        {
            _currentProblem = ((int) VirtualMachine.Instance.Ports.Input[0x3E80]) / 1000;
            UpdateMaxDistancePoint(Actuator.Actuator.Instance.Position.X, Actuator.Actuator.Instance.Position.Y);

            InvalidateVisual();
        }

        private void UpdateMaxDistancePoint(double sx, double sy)
        {
            if (Math.Sqrt(sx * sx + sy * sy) >
                Math.Sqrt(_maxDistancePoint.X * _maxDistancePoint.X + _maxDistancePoint.Y * _maxDistancePoint.Y))
                _maxDistancePoint = new Point(sx, sy);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Black, new Pen(Brushes.DarkRed, 0), new Rect(RenderSize));

            double centerX = RenderSize.Width / 2.0;
            double centerY = RenderSize.Height / 2.0;

            if (centerX == 0 || centerY == 0 || Actuator.Actuator.Instance.Track.Count == 0)
                return;

            EvalProblemAttributes(_currentProblem);

            double factor = Math.Min(centerX, centerY) /
                            Math.Sqrt(
                                _maxDistancePoint.X * _maxDistancePoint.X + _maxDistancePoint.Y * _maxDistancePoint.Y) /
                            1.2;

            factor *= AdditionalFactor;

            DrawInfo(drawingContext, factor);

            IList<Point> transformedPositions = TransformPositions(factor, centerX, centerY);

            // Линии сетки вокруг земли.
            DrawGrid(centerX, centerY, factor, drawingContext);

            // Рисуем землю.
            drawingContext.DrawEllipse(
                Brushes.DarkGray,
                new Pen(Brushes.LightGray, 1.0),
                new Point(centerX, centerY),
                6.357e6 * factor,
                6.357e6 * factor);

            // Траектория перелета.
            DrawTrack(drawingContext, transformedPositions);

            Point currentPosition = transformedPositions[transformedPositions.Count - 1];

            // Вектор скорости спутника.
            drawingContext.DrawLine(
                new Pen(Brushes.DarkGreen, 1.0),
                currentPosition,
                GetVector(currentPosition, Actuator.Actuator.Instance.DeltaSpeed)
                );

            // Рисуем части, зависимые от задачи.
            DrawProblemAttributes(new Point(centerX, centerY), drawingContext, factor);

            // Текущее положение спутника.
            if (transformedPositions.Count > 0)
            {
                drawingContext.DrawEllipse(
                    Brushes.DarkGreen,
                    new Pen(Brushes.LightGreen, 1.0),
                    currentPosition,
                    2.5,
                    2.5);
            }
        }

        private void DrawInfo(DrawingContext drawingContext, double factor)
        {
            drawingContext.DrawText(
                new FormattedText(
                    string.Format("Zoom: {0:e}x", factor),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Consolas"),
                    10,
                    Brushes.White),
                new Point(5, 5));

            // Рисуем скорость
            drawingContext.DrawText(
                new FormattedText(
                    string.Format("Vx: {0:e} km/s", Actuator.Actuator.Instance.Speed.X),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Consolas"),
                    10,
                    Brushes.White),
                new Point(5, 15));

            drawingContext.DrawText(
                new FormattedText(
                    string.Format("Vy: {0:e} km/s", Actuator.Actuator.Instance.Speed.Y),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Consolas"),
                    10,
                    Brushes.White),
                new Point(5, 25));

            drawingContext.DrawText(
                new FormattedText(
                    string.Format("V: {0:e} km/s", Actuator.Actuator.Instance.Speed.Length),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Consolas"),
                    10,
                    Brushes.White),
                new Point(5, 35));

            // Рисуем изменение скорости
            drawingContext.DrawText(
                new FormattedText(
                    string.Format("DVx: {0:e} km/s", Actuator.Actuator.Instance.DeltaSpeed.X),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Consolas"),
                    10,
                    Brushes.White),
                new Point(5, 45));

            drawingContext.DrawText(
                new FormattedText(
                    string.Format("DVy: {0:e} km/s", Actuator.Actuator.Instance.DeltaSpeed.Y),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Consolas"),
                    10,
                    Brushes.White),
                new Point(5, 55));

            drawingContext.DrawText(
                new FormattedText(
                    string.Format("DV: {0:e} km/s", Actuator.Actuator.Instance.DeltaSpeed.Length),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Consolas"),
                    10,
                    Brushes.White),
                new Point(5, 65));

            drawingContext.DrawText(
                new FormattedText(
                    string.Format("Distance to target orbit: {0:e} km", Actuator.Actuator.Instance.DistanceToTargetOrbit),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Consolas"),
                    10,
                    Brushes.White),
                new Point(5, 75));

            
        }

        private void EvalProblemAttributes(int problem)
        {
            switch (_currentProblem)
            {
                case 1:
                    double targetOrbit = VirtualMachine.Instance.Ports.Output[0x0004];

                    UpdateMaxDistancePoint(0, targetOrbit);

                    break;
            }
        }

        private void DrawProblemAttributes(Point center, DrawingContext context, double factor)
        {
            switch (_currentProblem)
            {
                case 1:
                    double targetOrbit = VirtualMachine.Instance.Ports.Output[0x0004];
                    targetOrbit *= factor;

                    context.DrawEllipse(
                        Brushes.Transparent, new Pen(Brushes.DarkRed, 1.5), center, targetOrbit, targetOrbit);

                    break;
            }
        }

        private Point GetVector(Point center, Vector vector)
        {
            double factor = Math.Log(vector.Length) * 5.0;

            double alpha = Math.Atan2(vector.X, vector.Y);

            double dx = factor * Math.Sin(alpha);
            double dy = factor * Math.Cos(alpha);

            return new Point(center.X + dx, center.Y + dy);
        }

        private void DrawTrack(DrawingContext drawingContext, IList<Point> transformedPositions)
        {
            if (transformedPositions.Count > 1)
            {
                for (int i = 1; i < transformedPositions.Count; ++i)
                {
                    drawingContext.DrawLine(
                        new Pen(Brushes.LightGreen, 1.0), transformedPositions[i - 1], transformedPositions[i]);
                }
            }
        }

        private void DrawGrid(double centerX, double centerY, double factor, DrawingContext drawingContext)
        {
            double invertFactor = 1 / factor;

            while (invertFactor > 3)
                invertFactor = invertFactor / 2;

            double gridStep = Math.Max(RenderSize.Width, RenderSize.Height) / 10.0 / invertFactor;

            for (int i = 0; i < 20; ++i)
            {
                drawingContext.DrawEllipse(
                    Brushes.Transparent,
                    new Pen(Brushes.LightBlue, 0.3),
                    new Point(centerX, centerY),
                    i * gridStep,
                    i * gridStep);
            }

            const double alphaStep = Math.PI * 2.0 / 16.0;
            var maxDimension = (int) Math.Max(RenderSize.Width, RenderSize.Height);

            for (double a = 0; a < 2 * Math.PI; a += alphaStep)
            {
                drawingContext.DrawLine(
                    new Pen(Brushes.LightBlue, 0.3),
                    new Point(centerX, centerY),
                    new Point(centerX + maxDimension * Math.Sin(a), centerY + maxDimension * Math.Cos(a)));
            }
        }

        private IList<Point> TransformPositions(double factor, double centerX, double centerY)
        {
            IList<Point> result = new List<Point>(Actuator.Actuator.Instance.Track.Count);

            foreach (Common.Point point in Actuator.Actuator.Instance.Track)
                result.Add(new Point(point.X * factor + centerX, point.Y * factor + centerY));

            return result;
        }

        private void Reset()
        {
            _maxDistancePoint = new Point(0, 0);
        }
    }
}
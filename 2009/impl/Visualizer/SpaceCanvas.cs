using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICFP2009.VirtualMachineLib;

namespace ICFP2009.Visualizer
{
    public class SpaceCanvas : Control
    {
        private readonly IList<Point> _previousPositions = new List<Point>();
        private int _currentProblem;
        private Point _maxDistancePoint = new Point(0, 0);
        private Point _speedVector = new Point(0, 0);

        static SpaceCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof (SpaceCanvas), new FrameworkPropertyMetadata(typeof (SpaceCanvas)));
        }

        public double AdditionalFactor { get; set; }

        public void StepCompleted()
        {
            double sx = -VirtualMachine.Instance.Ports.Output[0x0002];
            double sy = -VirtualMachine.Instance.Ports.Output[0x0003];

            _currentProblem = ((int) VirtualMachine.Instance.Ports.Input[0x3E80]) / 1000;

            _speedVector.X -= VirtualMachine.Instance.Ports.Input[0x0002];
            _speedVector.Y -= VirtualMachine.Instance.Ports.Input[0x0003];

            UpdateMaxDistancePoint(sx, sy);

            _previousPositions.Add(new Point(sx, sy));
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
            double centerX = RenderSize.Width / 2.0;
            double centerY = RenderSize.Height / 2.0;

            if (centerX == 0 || centerY == 0 || _previousPositions.Count == 0)
                return;

            EvalProblemAttributes(_currentProblem);

            double factor = Math.Min(centerX, centerY) /
                            Math.Sqrt(
                                _maxDistancePoint.X * _maxDistancePoint.X + _maxDistancePoint.Y * _maxDistancePoint.Y) /
                            1.2;

            factor *= AdditionalFactor;

            // Рисуем текущий масштаб.
            drawingContext.DrawText(
                new FormattedText(
                    string.Format("Zoom: {0:e}x", factor),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Consolas"),
                    10,
                    Brushes.Black),
                new Point(5, 5));

            // Рисуем скорость
            drawingContext.DrawText(
                new FormattedText(
                    string.Format("Vx: {0:e} km/s", _speedVector.X),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Consolas"),
                    10,
                    Brushes.Black),
                new Point(5, 15));

                        drawingContext.DrawText(
                new FormattedText(
                    string.Format("Vy: {0:e} km/s", _speedVector.Y),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Consolas"),
                    10,
                    Brushes.Black),
                new Point(5, 25));

            IList<Point> transformedPositions = TransformPositions(factor, centerX, centerY);

            // Линии сетки вокруг земли.
            DrawGrid(centerX, centerY, factor, drawingContext);

            // Рисуем землю.
            drawingContext.DrawEllipse(
                Brushes.DarkGray,
                new Pen(Brushes.LightGray, 1.0),
                new Point(centerX, centerY),
                5,
                5);

            // Траектория перелета.
            DrawTrack(drawingContext, transformedPositions);

            Point currentPosition = transformedPositions[transformedPositions.Count - 1];

            // Вектор скорости спутника.
            drawingContext.DrawLine(
                new Pen(Brushes.DarkGreen, 1.0),
                currentPosition,
                GetVector(currentPosition, _speedVector)
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
                    5,
                    5);
            }
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

        private Point GetVector(Point center, Point vector)
        {
            double factor = Math.Log(Math.Sqrt(_speedVector.X * _speedVector.X + _speedVector.Y * _speedVector.Y)) * 5.0;

            double alpha = Math.Atan2(_speedVector.X, _speedVector.Y);

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
                    new Pen(Brushes.LightBlue, 1.0),
                    new Point(centerX, centerY),
                    i * gridStep,
                    i * gridStep);
            }

            const double alphaStep = Math.PI * 2.0 / 16.0;
            var maxDimension = (int)Math.Max(RenderSize.Width, RenderSize.Height);

            for (double a = 0; a < 2 * Math.PI; a += alphaStep)
            {
                drawingContext.DrawLine(
                    new Pen(Brushes.LightBlue, 1.0),
                    new Point(centerX, centerY),
                    new Point(centerX + maxDimension * Math.Sin(a), centerY + maxDimension * Math.Cos(a)));
            }
        }

        private IList<Point> TransformPositions(double factor, double centerX, double centerY)
        {
            IList<Point> result = new List<Point>(_previousPositions.Count);

            foreach (Point point in _previousPositions)
                result.Add(new Point(point.X * factor + centerX, point.Y * factor + centerY));

            return result;
        }

        public void Reset()
        {
            _previousPositions.Clear();
            _speedVector = new Point(0, 0);
            _maxDistancePoint = new Point(0, 0);
        }
    }
}
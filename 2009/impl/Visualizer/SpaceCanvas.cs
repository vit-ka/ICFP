using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICFP2009.VirtualMachineLib;

namespace ICFP2009.Visualizer
{
    public class SpaceCanvas : Control
    {
        private readonly IList<Point> _previousPositions = new List<Point>();
        private Point _maxDistancePoint = new Point(0, 0);
        private Point _speedVector = new Point(0, 0);
        private Size _size;

        static SpaceCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof (SpaceCanvas), new FrameworkPropertyMetadata(typeof (SpaceCanvas)));
        }

        public void StepCompleted()
        {
            double sx = VirtualMachine.Instance.Ports.Output[0x0002];
            double sy = VirtualMachine.Instance.Ports.Output[0x0003];

            _speedVector.X -= VirtualMachine.Instance.Ports.Input[0x0002];
            _speedVector.Y -= VirtualMachine.Instance.Ports.Input[0x0003];

            if (Math.Sqrt(sx * sx + sy * sy) >
                Math.Sqrt(_maxDistancePoint.X * _maxDistancePoint.X + _maxDistancePoint.Y * _maxDistancePoint.Y))
                _maxDistancePoint = new Point(sx, sy);

            _previousPositions.Add(new Point(sx, sy));
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            _size = sizeInfo.NewSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            double centerX = _size.Width / 2.0;
            double centerY = _size.Height / 2.0;

            if (centerX == 0 || centerY == 0)
                return;

            double factor = Math.Min(centerX, centerY) /
                            Math.Sqrt(
                                _maxDistancePoint.X * _maxDistancePoint.X + _maxDistancePoint.Y * _maxDistancePoint.Y) /
                            1.2;
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

            double gridStep = Math.Max(_size.Width, _size.Height) / 10.0 / invertFactor;
            
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
            var maxDimension = (int) Math.Max(_size.Width, _size.Height);
            
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
    }
}
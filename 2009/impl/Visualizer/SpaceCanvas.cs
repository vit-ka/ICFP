using System;
using System.Collections;
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
        private Size size;

        static SpaceCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof (SpaceCanvas), new FrameworkPropertyMetadata(typeof (SpaceCanvas)));
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            size = sizeInfo.NewSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            double sx = VirtualMachine.Instance.Ports.Output[0x0002];
            double sy = VirtualMachine.Instance.Ports.Output[0x0003];

            double centerX = size.Width / 2.0;
            double centerY = size.Height / 2.0;
            
            if (centerX == 0 || centerY == 0)
                return;

            _previousPositions.Add(new Point(sx, sy));

            double factor = Math.Min(centerX, centerY) / Math.Sqrt(sx * sx + sy * sy);
            IList<Point> transformedPositions = TransformPositions(factor, centerX, centerY);


            drawingContext.DrawEllipse(
                Brushes.DarkGray,
                new Pen(Brushes.LightGray, 1.0),
                new Point(centerX, centerY),
                5,
                5);

            if (transformedPositions.Count > 1)
            for (int i = 1; i < transformedPositions.Count; ++i )
            {
                drawingContext.DrawLine(new Pen(Brushes.LightGreen, 1.0), transformedPositions[i-1], transformedPositions[i]);
            }
                
            drawingContext.DrawEllipse(Brushes.DarkGreen,
                new Pen(Brushes.LightGreen, 1.0),
                transformedPositions[transformedPositions.Count - 1],
                5,
                5);
            
        }

        private IList<Point> TransformPositions(double factor, double centerX, double centerY)
        {
            IList<Point> result = new List<Point>(_previousPositions.Count);

            foreach (Point point in _previousPositions)
            {
                result.Add(new Point(point.X * factor + centerX, point.Y * factor + centerY));
            }

            return result;
        }
    }
}
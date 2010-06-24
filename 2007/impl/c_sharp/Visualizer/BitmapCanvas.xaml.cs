using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Visualizer
{
    /// <summary>
    /// Interaction logic for BitmapCanvas.xaml
    /// </summary>
    public partial class BitmapCanvas : Canvas
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public BitmapCanvas()
        {
            InitializeComponent();
        }

        /// <summary>
        /// RNA runner.
        /// </summary>
        public RnaRunner.RnaRunner RnaRunner { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        protected override void OnRender(DrawingContext dc)
        {
            if (RnaRunner == null)
                return;

            var bitmapSource = BitmapConverter.Convert(RnaRunner.PixelMap);

            dc.DrawImage(bitmapSource, new Rect(0, 0, 600, 600));
        }
    }
}
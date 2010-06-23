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

            const int stride = 600 * 3 + (600 * 3) % 4;

            var bits = new byte[600 * stride];

            for (int x = 0; x < 600; ++x)
                for (int y = 0; y < 600; ++y)
                {
                    bits[x * 3 + y * stride] = RnaRunner.Bitmap[x, y].Color.R;
                    bits[x*3 + y*stride + 1] = RnaRunner.Bitmap[x, y].Color.G;
                    bits[x*3 + y*stride + 2] = RnaRunner.Bitmap[x, y].Color.B;
                }

            BitmapSource bitmapSource = BitmapSource.Create(
                600, 600,
                300, 300,
                PixelFormats.Rgb24,
                null,
                bits,
                stride);

            dc.DrawImage(bitmapSource, new Rect(0, 0, 600, 600));
        }
    }
}
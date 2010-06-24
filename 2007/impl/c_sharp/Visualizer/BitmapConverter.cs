using System.Windows.Media;
using System.Windows.Media.Imaging;
using RnaRunner;

namespace Visualizer
{
    internal class BitmapConverter
    {
        public static BitmapSource Convert(PixelMap map)
        {
            const int stride = 600 * 3 + (600 * 3) % 4;

            var bits = new byte[600 * stride];

            for (int x = 0; x < 600; ++x)
                for (int y = 0; y < 600; ++y)
                {
                    bits[x * 3 + y * stride] = map[x, y].Color.R;
                    bits[x * 3 + y * stride + 1] = map[x, y].Color.G;
                    bits[x * 3 + y * stride + 2] = map[x, y].Color.B;
                }

            BitmapSource bitmapSource = BitmapSource.Create(
                600, 600,
                300, 300,
                PixelFormats.Rgb24,
                null,
                bits,
                stride);

            return bitmapSource;
        }
    }
}
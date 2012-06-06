namespace RnaRunner
{
    /// <summary>
    /// Map of pixels.
    /// </summary>
    public class PixelMap
    {
        private Pixel[,] _map;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PixelMap()
        {
            _map = new Pixel[600,600];
        }

        /// <summary>
        /// Indexator of pixels.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>Pixel at given position.</returns>
        public Pixel this[int x, int y]
        {
            get { return _map[x, y]; }
            set { _map[x, y] = value;}
        }
    }
}
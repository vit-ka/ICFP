using System.Drawing;

namespace RnaRunner
{
    /// <summary>
    /// Pixel of map.
    /// </summary>
    public class Pixel
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="color">Color of pixel.</param>
        /// <param name="transparency">Transperency of pixel.</param>
        public Pixel(Color color, byte transparency)
        {
            Color = color;
            Transparency = transparency;
        }

        /// <summary>
        /// Color of pixel.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Transperency of pixel.
        /// </summary>
        public byte Transparency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Pixel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Color.Equals(Color) && other.Transparency == Transparency;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Pixel)) return false;
            return Equals((Pixel) obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Color.GetHashCode()*397) ^ Transparency.GetHashCode();
            }
        }
    }
}
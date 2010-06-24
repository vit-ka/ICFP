using System;
using System.Drawing;

namespace RnaRunner
{
    /// <summary>
    /// Pixel of map.
    /// </summary>
    public class Pixel : IEquatable<Pixel>
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
            return other.Color.R.Equals(Color.R) && other.Color.G.Equals(Color.G) && other.Color.B.Equals(Color.B) &&
                   other.Transparency == Transparency;
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
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Pixel left, Pixel right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Pixel left, Pixel right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Color.GetHashCode() * 397) ^ Transparency.GetHashCode();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Pixel Clone()
        {
            var result = new Pixel(Color.FromArgb(Color.R, Color.G, Color.B), Transparency);
            return result;
        }
    }
}
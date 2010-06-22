using System;

namespace RopeStrings
{
    internal class RopeSector
    {
        private const int _sectorSize = 50;

        private readonly char[] _chars = new char[_sectorSize];

        /// <summary>
        /// Size of sector in chars.
        /// </summary>
        public static int SectorSize
        {
            get
            {
                return _sectorSize;
            }
        }

        /// <summary>
        /// Count of items.
        /// </summary>
        public int Count { get; set; }

        public char this[int index]
        {
            get
            {
                return _chars[index];
            }

            set
            {
                _chars[index] = value;
            }
        }

        public void RemoveFromBegin(int remainsToRemove)
        {
            for (int index = 0; index + remainsToRemove < Count; ++index)
            {
                _chars[index] = _chars[index + remainsToRemove];
            }

            Count -= remainsToRemove;
        }

        public char[] ToCharArray()
        {
            var result = new char[Count];
            Array.Copy(_chars, 0, result, 0, Count);
            return result;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("Chars: {0}", new string(_chars, 0, Count));
        }
    }
}
using System;

namespace RopeStrings
{
    internal class RopeSector
    {
        private const int _sectorSize = 50;
        private char[] _chars;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public RopeSector()
        {
            Length = 0;
            _chars = new char[SectorSize];
        }

        public char this[int index]
        {
            get
            {
                return _chars[index];
            }
        }

        public void AppendToBack(char ch)
        {
            _chars[Length++] = ch;
        }

        public static int SectorSize
        {
            get
            {
                return _sectorSize;
            }
        }

        public int Length
        {
            get; private set;
        }

        public char[] ToCharArray()
        {
            return _chars;
        }

        public override string ToString()
        {
            return new string(_chars, 0, Length);
        }
    }
}
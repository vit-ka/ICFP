using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RopeStrings
{
    /// <summary>
    /// Represents string with rope behaviour.
    /// </summary>
    public class RopeString
    {
        private class RopeSectorDescriptor
        {
            public RopeSector Sector { get; set; }
            public int FirstIndexInGlobal { get; set; }

            public override string ToString()
            {
                return string.Format("{0}: {1}", FirstIndexInGlobal, Sector);
            }
        }

        private List<RopeSectorDescriptor> _descriptors;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="str">Initialization string.</param>
        public RopeString(string str)
        {
            var ropeString = ConvertFromRegularString(str);
            _descriptors = ropeString._descriptors;
            Length = ropeString.Length;
        }

        private RopeString()
        {
            _descriptors = new List<RopeSectorDescriptor>();
        }

        private static RopeString ConvertFromRegularString(string regularString)
        {
            var ropeString = new RopeString();

            int currentIndex = 0;
            while (currentIndex < regularString.Length)
            {
                var sector = new RopeSector();
                while (sector.Length < RopeSector.SectorSize && currentIndex < regularString.Length)
                {
                    sector.AppendToBack(regularString[currentIndex++]);
                }

                ropeString.PushSectorToBack(sector);
            }

            return ropeString;
        }

        private void PushSectorToBack(RopeSector sector)
        {
            _descriptors.Add(
                new RopeSectorDescriptor
                    {
                        FirstIndexInGlobal = Length,
                        Sector = sector
                    });
            Length += sector.Length;
        }

        /// <summary>
        /// Length of string.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Append another rope string to back of current.
        /// </summary>
        /// <param name="ropeString">Another rope string.</param>
        /// <param name="fromIndex">Index in another string from we append.</param>
        public void AppendToBack(RopeString ropeString, int fromIndex)
        {
            // First we search sector with lesser or equals first index to fromIndex
            int anotherStringSectorIndex = SearchSectorByGlobalIndex(ropeString, fromIndex);
         
            if (anotherStringSectorIndex == -1)
                throw new InvalidOperationException("fromIndex ouside of bounds");

            // Cope from found sector from fromIndex to end to another new sector.
            var firstNewSector = new RopeSector();
            var firstOldSector = ropeString._descriptors[anotherStringSectorIndex].Sector;
            var indexInsideFirstSector = fromIndex -
                                         ropeString._descriptors[anotherStringSectorIndex].FirstIndexInGlobal;

            // If index inside sector is zero --- just copy full sector.
            if (indexInsideFirstSector > 0)
            {
                while (indexInsideFirstSector < firstOldSector.Length)
                {
                    firstNewSector.AppendToBack(firstOldSector[indexInsideFirstSector++]);
                }

                // Add new first sector to back of current string.
                _descriptors.Add(
                    new RopeSectorDescriptor
                        {
                            FirstIndexInGlobal = Length,
                            Sector = firstNewSector
                        });
                Length += firstNewSector.Length;
                ++anotherStringSectorIndex;
            }

            // Batch add all next sectors to back of currect string.
            while (anotherStringSectorIndex < ropeString._descriptors.Count)
            {
                RopeSector sector = ropeString._descriptors[anotherStringSectorIndex++].Sector;
                _descriptors.Add(
                    new RopeSectorDescriptor
                        {
                            FirstIndexInGlobal = Length,
                            Sector = sector
                        });
                Length += sector.Length;
            }
        }

        private int SearchSectorByGlobalIndex(RopeString ropeString, int fromIndex)
        {
            var estimateSectorIndex = fromIndex / RopeSector.SectorSize;

            int stepSign;

            if (ropeString._descriptors[estimateSectorIndex].FirstIndexInGlobal > fromIndex)
                stepSign = -1;
            else
            {
                stepSign = 1;
            }

            while (estimateSectorIndex >= 0 && estimateSectorIndex < ropeString._descriptors.Count)
            {
                if (ropeString._descriptors[estimateSectorIndex].FirstIndexInGlobal <= fromIndex &&
                    ropeString._descriptors[estimateSectorIndex].FirstIndexInGlobal +
                    ropeString._descriptors[estimateSectorIndex].Sector.Length > fromIndex)
                    return estimateSectorIndex;

                estimateSectorIndex += stepSign;
            }

            return -1;
        }

        /// <summary>
        /// Has string pattern at given position?
        /// </summary>
        /// <param name="pattern">Pattern for search.</param>
        /// <param name="fromIndex">Index of pattern.</param>
        /// <returns>True if has.</returns>
        public bool HasPatternAtPosition(string pattern, int fromIndex)
        {
            int anotherStringSectorIndex = SearchSectorByGlobalIndex(this, fromIndex);

            if (anotherStringSectorIndex == -1)
                throw new InvalidOperationException("fromIndex outside of bounds");

            if (Length < fromIndex + pattern.Length)
                return false;

            var indexInsideSector = fromIndex - _descriptors[anotherStringSectorIndex].FirstIndexInGlobal;
            var sector = _descriptors[anotherStringSectorIndex].Sector;

            if (sector.Length - indexInsideSector < pattern.Length)
            {
                // Check first sector.
                int indexInsidePattern = 0;

                while (indexInsidePattern < pattern.Length && indexInsideSector < RopeSector.SectorSize)
                {
                    if (pattern[indexInsidePattern++] != sector[indexInsideSector++])
                        return false;
                }

                // Check another sectors.
                while (indexInsidePattern < pattern.Length && anotherStringSectorIndex + 1 < _descriptors.Count)
                {
                    sector = _descriptors[anotherStringSectorIndex++].Sector;
                    indexInsideSector = 0;

                    while (indexInsidePattern < pattern.Length && indexInsideSector < RopeSector.SectorSize)
                    {
                        if (pattern[indexInsidePattern++] != sector[indexInsideSector++])
                            return false;
                    }
                }

                if (anotherStringSectorIndex == _descriptors.Count - 1 || indexInsidePattern < pattern.Length)
                    return false;

                return true;
            }
            
            
            return pattern.All(t => sector[indexInsideSector++] == t);
        }

        /// <summary>
        /// Evaluate index of first occurs search pattern in string.
        /// </summary>
        /// <param name="searchPattern">Pattern to search.</param>
        /// <param name="fromIndex">Start index for search.</param>
        /// <returns>Index of first occurs or -1.</returns>
        public int IndexOf(string searchPattern, int fromIndex)
        {
            return -1;
        }

        /// <summary>
        /// Returns substring of current string.
        /// </summary>
        /// <param name="fromIndex">Index of substring start.</param>
        /// <param name="length">Length of substring.</param>
        /// <returns>Substring.</returns>
        public RopeString Substring(int fromIndex, int length)
        {
            return new RopeString(string.Empty);
        }

        /// <summary>
        /// Convert current string to regular string.
        /// </summary>
        /// <returns>Regular string.</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var ropeSectorDescriptor in _descriptors)
            {
                builder.Append(ropeSectorDescriptor.Sector.ToCharArray(), 0, ropeSectorDescriptor.Sector.Length);
            }

            return builder.ToString();
        }
    }
}
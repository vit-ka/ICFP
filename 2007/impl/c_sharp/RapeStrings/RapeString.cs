using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RapeStrings
{
    /// <summary>
    /// Class for representing rape-behaviour of string.
    /// </summary>
    public class RapeString
    {
        private int _count;
        private readonly LinkedList<RapeSector> _sectors;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public RapeString()
        {
            _sectors = new LinkedList<RapeSector>();
            _sectors.AddFirst(new RapeSector());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <param name="initializeString">Initialization string.</param>
        public RapeString(string initializeString)
        {
            _sectors = new LinkedList<RapeSector>();

            char[] chars = initializeString.ToCharArray();

            for (int index = 0; index < chars.Length; index += RapeSector.SectorSize)
            {
                var sector = new RapeSector();
                for (int innerIndex = 0; innerIndex < RapeSector.SectorSize && index + innerIndex < chars.Length; ++innerIndex)
                    sector[innerIndex] = chars[index + innerIndex];
                sector.Count = index + RapeSector.SectorSize < chars.Length
                                   ? RapeSector.SectorSize
                                   : chars.Length - index;
                _sectors.AddLast(sector);
            }

            _count = chars.Length;
            CheckCount();
        }

        /// <summary>
        /// Length of string.
        /// </summary>
        public int Length
        {
            get
            {
                return _count;
            }
        }

        private void CheckCount()
        {
            int temp = 0;
            foreach (var rapeSector in _sectors)
            {
                temp += rapeSector.Count;
            }

            if (temp != _count)
            {
                File.WriteAllText("D:\\123.txt", "aaaa!");
            }
        }

        /// <summary>
        /// Access to element at index.
        /// </summary>
        /// <param name="index">Index of element.</param>
        /// <returns>Element at index.</returns>
        public char this[int index]
        {
            get
            {
                int currentFirstIndex = 0;

                LinkedListNode<RapeSector> currentNode = _sectors.First;
                while (currentNode != null && currentNode.Value.Count + currentFirstIndex < index)
                {
                    currentFirstIndex += currentNode.Value.Count;
                    currentNode = currentNode.Next;
                }

                if (currentNode != null)
                    return currentNode.Value[index - currentFirstIndex];

                throw new IndexOutOfRangeException();
            }
        }

        ///<summary>
        /// Checks what string starts with pattern.
        ///</summary>
        ///<param name="pattern">Pattern.</param>
        ///<returns>True if starts</returns>
        public bool StartsWith(string pattern)
        {
            if (_count < pattern.Length)
                return false;

            int index = 0;
            int indexInNode = 0;
            LinkedListNode<RapeSector> currentNode = _sectors.First;
            while (index < pattern.Length)
            {
                if (indexInNode >= currentNode.Value.Count)
                {
                    indexInNode = 0;
                    currentNode = currentNode.Next;
                }

                if (currentNode.Value[indexInNode] != pattern[index])
                    return false;

                ++index;
                ++indexInNode;
            }

            return true;
        }

        /// <summary>
        /// Removes char for begin of string.
        /// </summary>
        /// <param name="count">Count of removed chars.</param>
        public void RemoveFromBegin(int count)
        {
            int remainsToRemove = count;
            LinkedListNode<RapeSector> currentNode = _sectors.First;

            while (remainsToRemove > 0)
            {
                if (currentNode.Value.Count <= remainsToRemove)
                {
                    remainsToRemove -= currentNode.Value.Count;
                    _sectors.RemoveFirst();
                    currentNode = _sectors.First;
                }
                else
                {
                    currentNode.Value.RemoveFromBegin(remainsToRemove);
                    remainsToRemove = 0;
                }
            }
        }

        /// <summary>
        /// Returns substring from given index and given length.
        /// </summary>
        /// <param name="from">Index from.</param>
        /// <param name="length">Length of substring.</param>
        public RapeString Substring(int from, int length)
        {
            int currentFirstIndex = 0;
            LinkedListNode<RapeSector> currentNode = _sectors.First;
            while (currentNode != null && currentNode.Value.Count + currentFirstIndex < from)
            {
                currentFirstIndex += currentNode.Value.Count;
                currentNode = currentNode.Next;
            }

            var result = new RapeString();

            if (currentNode != null)
            {
                int currentLength = 0;

                // Copy from first node.
                var copyLen = Math.Min(currentNode.Value.Count - (from - currentFirstIndex), length);
                var firstNodePart = new char[copyLen];
                Array.Copy(currentNode.Value.ToCharArray(), from - currentFirstIndex, firstNodePart, 0, copyLen);

                result.Append(firstNodePart);
                currentLength += copyLen;
                currentNode = currentNode.Next;

                while (currentLength < length && currentNode != null)
                {
                    if (currentLength + currentNode.Value.Count > length)
                    {
                        var nodePart = new char[length - currentLength];
                        Array.Copy(currentNode.Value.ToCharArray(), nodePart, length - currentLength);

                        result.Append(nodePart);
                        currentLength += nodePart.Length;
                    }

                    result.Append(currentNode.Value);
                    currentLength += currentNode.Value.Count;
                    currentNode = currentNode.Next;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds values to the end of string.
        /// </summary>
        /// <param name="str">Values to add</param>
        public void Append(string str)
        {
            Append(str.ToCharArray());
        }

        /// <summary>
        /// Adds values to the end of string.
        /// </summary>
        /// <param name="str">Values to add</param>
        public void Append(RapeString str)
        {
            foreach (var rapeSector in str._sectors)
            {
                _sectors.AddLast(rapeSector);
                _count += rapeSector.Count;
            }
            CheckCount();
        }

        private void Append(RapeSector value)
        {
            _sectors.AddLast(value);
            _count += value.Count;
            CheckCount();
        }

        private void Append(char[] charArray)
        {
            LinkedListNode<RapeSector> currentNode = _sectors.Last;
            int currentIndex = currentNode.Value.Count;
            int indexInArray = 0;

            while (indexInArray < charArray.Length)
            {
                if (currentIndex >= RapeSector.SectorSize)
                {
                    currentIndex = 0;
                    currentNode = _sectors.AddLast(new RapeSector());
                }
                currentNode.Value[currentIndex] = charArray[indexInArray];
                ++currentNode.Value.Count;
                ++currentIndex;
                ++indexInArray;
                ++_count;
            }
        }

        /// <summary>
        /// Adds values to the end of string.
        /// </summary>
        /// <param name="symbol">Values to add</param>
        public void Append(char symbol)
        {
            Append(
                new[]
                    {
                        symbol
                    });
        }

        /// <summary>
        /// Add content to the begin of string.
        /// </summary>
        /// <param name="str">Value to adding.</param>
        public void AddFirst(RapeString str)
        {
            LinkedListNode<RapeSector> currentNode = str._sectors.Last;

            while (currentNode != null)
            {
                _sectors.AddFirst(currentNode.Value);
                _count += currentNode.Value.Count;
                currentNode = currentNode.Previous;
            }

            CheckCount();
        }

        /// <summary>
        /// Add content to the begin of string.
        /// </summary>
        /// <param name="str">Value to adding.</param>
        public void AddFirst(string str)
        {
            var chars = str.ToCharArray();

            int countSectors = chars.Length / RapeSector.SectorSize;
            int charIndex = 0;

            LinkedListNode<RapeSector> currentSector = null;

            for (int index = 0; index < countSectors; ++index)
            {
                var sector = new RapeSector();
                for (int innerIndex = 0; innerIndex < RapeSector.SectorSize; ++innerIndex, ++charIndex)
                {
                    sector[innerIndex] = chars[charIndex];
                }
                sector.Count = RapeSector.SectorSize;

                if (currentSector == null)
                    currentSector = _sectors.AddFirst(sector);
                else
                    _sectors.AddAfter(currentSector, sector);

                _count += sector.Count;
            }

            CheckCount();

            var lastSector = new RapeSector();
            for (int innerIndex = 0; charIndex < chars.Length; ++innerIndex, ++charIndex)
            {
                lastSector[innerIndex] = chars[charIndex];
            }
            lastSector.Count = chars.Length % RapeSector.SectorSize;

            if (currentSector == null)
                _sectors.AddFirst(lastSector);
            else
                _sectors.AddAfter(currentSector, lastSector);
            _count += lastSector.Count;

            CheckCount();
        }

        /// <summary>
        /// Search substring staring with given position.
        /// </summary>
        /// <param name="searchPattern">Pattern to search.</param>
        /// <param name="startIndex">Start index to search.</param>
        /// <returns>Index of first given pattern.</returns>
        public int IndexOf(string searchPattern, int startIndex)
        {
            int currentFirstIndex = 0;
            LinkedListNode<RapeSector> currentNode = _sectors.First;

            while (currentNode != null && currentNode.Value.Count + currentFirstIndex < startIndex)
            {
                currentFirstIndex += currentNode.Value.Count;
                currentNode = currentNode.Next;
            }
            
            if (currentNode == null)
                return -1;

            int index = startIndex - currentFirstIndex;
                    
            while (currentNode != null)
            {
                if (index >= currentNode.Value.Count)
                {
                    index = 0;
                    currentFirstIndex += currentNode.Value.Count;
                    currentNode = currentNode.Next;

                    continue;
                }

                if (!IsPatternCompare(index, searchPattern, currentFirstIndex, currentNode))
                {
                    ++index;
                }
                else
                {
                    return currentFirstIndex + index;
                }
            }

            return -1;
        }

        private bool IsPatternCompare(int index, string searchPattern, int firstIndexOfNode, LinkedListNode<RapeSector> currentNode)
        {
            int indexInNode = index;
            int indexInPattern = 0;

            while (indexInPattern < searchPattern.Length && currentNode != null)
            {
                if (indexInNode >= currentNode.Value.Count)
                {
                    currentNode = currentNode.Next;
                    indexInNode = 0;
                }

                if (currentNode.Value[indexInNode] != searchPattern[indexInPattern])
                    return false;

                ++indexInNode;
                ++indexInPattern;
            }

            return currentNode != null;
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
            var builder = new StringBuilder(_count);

            foreach (var rapeSector in _sectors)
            {
                builder.Append(rapeSector.ToCharArray());
            }

            return builder.ToString();
        }
    }

    internal class RapeSector
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
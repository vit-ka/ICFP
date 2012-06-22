using System;
using System.Collections.Generic;

namespace UM.Command
{
    public class MemoryManager
    {
        private static MemoryManager instance;
        private IList<uint[]> arrays;

        public static MemoryManager GetInstance()
        {
            if (instance == null)
                instance = new MemoryManager();

            return instance;
        }

        public MemoryManager()
        {
            arrays = new List<uint[]> {null};
        }

        public uint Allocate(uint aCapasity)
        {
            uint[] newArray = new uint[aCapasity];

            for (int i = 0; i < aCapasity; ++i)
                newArray[i] = 0;

            uint newArrayID = getFreeArrayID();
            if (arrays.Count == newArrayID)
                arrays.Add(null);

            arrays[(int) newArrayID] = newArray;
            return newArrayID;
        }

        public void Abandon(uint anArrayID)
        {
            arrays[(int) anArrayID] = null;
        }

        private uint getFreeArrayID()
        {
            uint arraysSize = (uint) arrays.Count;

            for (uint i = 0; i < arraysSize; ++i)
                if (arrays[(int) i] == null)
                {
                    return i;
                }

            return arraysSize;
        }

        public void CopyToZeroArray(uint anArrayID)
        {
            if (anArrayID == 0)
                return;

            if (arrays[0] != null)
            {
                arrays[0] = null;
            }

            arrays[0] = new uint[arrays[(int) anArrayID].Length];
            Array.Copy(arrays[(int) anArrayID], arrays[0], arrays[(int) anArrayID].Length);
        }

        public void LoadScroll(uint[] aScroll)
        {
            if (arrays[0] != null)
            {
                arrays[0] = null;
            }

            arrays[0] = new uint[aScroll.Length];
            Array.Copy(aScroll, arrays[0], aScroll.Length);
        }

        public uint this[uint i, uint currentOffset]
        {
            get { return arrays[(int)i][currentOffset]; }
            set { arrays[(int)i][currentOffset] = value; }
        }
    }
}
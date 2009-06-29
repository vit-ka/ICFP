using System;
using System.Collections.Generic;

namespace ICFP2009.VirtualMachineLib
{
    internal class MemoryManager
    {
        private readonly double[] _memory;

        public MemoryManager(List<double> initialMemory)
        {
            _memory = initialMemory.ToArray();
        }

        public double this[Int16 index]
        {
            get
            {
                return _memory[index];
            }
            set
            {
                _memory[index] = value;
            }
        }
    }
}
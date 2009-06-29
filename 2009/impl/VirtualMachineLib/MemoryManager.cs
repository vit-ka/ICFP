using System.Collections.Generic;

namespace ICFP2009.VirtualMachineLib
{
    internal class MemoryManager
    {
        private double[] _memory;

        public MemoryManager(List<double> initialMemory)
        {
            _memory = initialMemory.ToArray();
        }
    }
}
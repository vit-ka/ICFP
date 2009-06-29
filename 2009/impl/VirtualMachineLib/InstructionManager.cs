using System.Collections.Generic;

namespace ICFP2009.VirtualMachineLib
{
    internal class InstructionManager
    {
        private int[] _instructions;

        public InstructionManager(List<int> instructions)
        {
            _instructions = instructions.ToArray();
            
        }
    }
}
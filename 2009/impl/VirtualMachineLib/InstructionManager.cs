using System;
using System.Collections.Generic;

namespace ICFP2009.VirtualMachineLib
{
    internal class InstructionManager
    {
        private Int32[] _instructions;
        private Int16 _currentInstruction;
        private bool _statusRegister;

        public InstructionManager(List<Int32> instructions)
        {
            _instructions = instructions.ToArray();
            
        }
    }
}
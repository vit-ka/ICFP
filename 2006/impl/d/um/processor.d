module um.processor;

import std.string;
import std.stdio;
import um.memory;

class Processor
{
	 private:
    
			uint[] _registers;
			bool _isHalted = false;

			uint _finger = 0;

			Memory _memory;

			static Processor _instance;

			this()
			{
				 _registers = new uint[8];
				 _memory = Memory.getInstance();
			}

	 public:

			static Processor getInstance()
			{
				 if (_instance is null)
						_instance = new Processor();

				 return _instance;
			}

			invariant(char)[] toString()
			{
				 invariant(char)[] result = 
						this.classinfo.name 
						~ "@" 
						~ std.string.toString(this.toHash()) 
						~ "[";

				 result ~= "registers={";

				 foreach (int index, uint reg; _registers)
				 {
						result ~= std.string.toString(reg);

						if (index != _registers.length - 1)
							 result ~= ", ";
				 }

				 result ~= "}, halted=" ~ std.string.toString(_isHalted);

				 return result ~ "]";
			}

			void performProgram()
			{
				 while (!_isHalted)
				 {
						uint instruction = _memory.data(0)[_finger++];
  
						byte type = (instruction & 0xF0000000) >> 28;
  
						byte regA = (instruction & 0x000001C0) >> 6;
						byte regB = (instruction & 0x00000038) >> 3;
						byte regC =  instruction & 0x00000007;
  
						//writefln("%2x", type);
  
						switch (type)
						{
							 // Conditional move
							 case 0x00:
							 {
									if (_registers[regC] != 0)
										 _registers[regA] = _registers[regB];
  
									break;
							 }
  
							 // Array index
							 case 0x01:
							 {
									_registers[regA] = _memory.data(_registers[regB])[_registers[regC]];
      
									break;
							 }
  
							 // Array amendment
							 case 0x02:
							 {
									*(_memory.data(_registers[regA]) + _registers[regB]) = _registers[regC];
      
									break;
							 }
  
							 // Addition
							 case 0x03:
							 {
									_registers[regA] = _registers[regB] + _registers[regC];
      
									break;
							 }
  
							 // Multiplication
							 case 0x04:
							 {
									_registers[regA] = _registers[regB] * _registers[regC];
  
									break;
							 } 
      
							 // Division
							 case 0x05:
							 {
									_registers[regA] = _registers[regB] / _registers[regC];
  
									break;
							 } 
  
							 // Not and
							 case 0x06:
							 {
									_registers[regA] = ~(_registers[regB] & _registers[regC]);
  
									break;
							 }
  
							 // Halt
							 case 0x07:
							 {
									_isHalted = true;
									writefln("\n\nProgram halted by command.\n");
									return;
							 }
  
							 // Allocation
							 case 0x08:
							 {
									_registers[regB] = _memory.allocate(_registers[regC]);
  
									break;
							 }
    
							 // Abandonmen
							 case 0x09:
							 {
									_memory.abandon(_registers[regC]);
  
									break;
							 }
  
							 // Output
							 case 0x0a:
							 {
									writef(cast(char)_registers[regC]);
  
									break;
							 }
  
							 // Input
							 case 0x0b:
							 {
									byte value = getchar();
          
									if (value > 127)
										 _registers[regC] = 0xffffffff;
									else
										 _registers[regC] = value;
  
									break;
							 }
  
							 // Load program
							 case 0x0c:
							 {
									_memory.copyToZeroArray(_registers[regB]);
									_finger = _registers[regC];
  
									break;
							 }
  
							 // Orphographi
							 case 0x0d:
							 {
									byte reg   = (instruction & 0x0E000000) >> 25;
									uint value =  instruction & 0x01FFFFFF;
  
									_registers[reg] = value;
									break;
							 }
  
							 default:
							 {
									writefln("Illegal code: %d", type);
									_isHalted = true;
									return;
							 }
						}
				 }
			}
}

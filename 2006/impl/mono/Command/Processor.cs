using System;

namespace UM.Command
{
	public class Processor
	{
		private static Processor instance;
		private MemoryManager memory;
		private readonly uint[] registers;
		public const int REGISTERS_COUNT = 8;
		private bool halted = false;

		private uint currentOffset = 0;

		public static Processor GetInstance()
		{
			if (instance == null)
				instance = new Processor();

			return instance;
		}

		private Processor()
		{
			registers = new uint[REGISTERS_COUNT];
			for (int i = 0; i < REGISTERS_COUNT; i++)
				registers[i] = 0;

			memory = MemoryManager.GetInstance();
		}

		public void Clear()
		{
			for (int i = 0; i < REGISTERS_COUNT; ++i)
				registers[i] = 0;
		}

		public void MoveCursorTo(uint anOffset)
		{
			currentOffset = anOffset;
		}

		public void PerformProgram()
		{
    		while (!halted)
    		{
    			uint instruction = memory[0, currentOffset];
    			currentOffset++;

    			InstructionType instructionType =
    				(InstructionType) ((instruction & 0xF0000000L) >> 28);
    			
    			byte registerAIndex = (byte)((instruction & 0x000001C0L) >> 6);
    			byte registerBIndex = (byte)((instruction & 0x00000038L) >> 3);
    			byte registerCIndex = (byte)(instruction & 0x00000007L);
    			

    			switch (instructionType)
    			{
    				case InstructionType.CONDITIONAL_MOVE:
    					{
    						if (registers[registerCIndex] != 0)
    							registers[registerAIndex] = registers[registerBIndex];
    						break;
    					}

    				case InstructionType.ARRAY_INDEX:
    					{
    						registers[registerAIndex] =
    							memory[
    								registers[registerBIndex],
    								registers[registerCIndex]];

    						break;
    					}
    				case InstructionType.ARRAY_AMENDMENT:
    					{
    						memory[registers[registerAIndex],
    								registers[registerBIndex]] =
    								registers[registerCIndex];
    						break;
    					}
    				case InstructionType.ADDITION:
    					{
    						uint operand1 = registers[registerBIndex];
    						uint operand2 = registers[registerCIndex];
    						registers[registerAIndex] = operand1 + operand2;
    						break;
    					}
    				case InstructionType.MULTIPLICATION:
    					{
    						uint operand1 = registers[registerBIndex];
    						uint operand2 = registers[registerCIndex];
    						registers[registerAIndex] = operand1 * operand2;
    						break;
    					}
    				case InstructionType.DIVISION:
    					{
    						uint operand1 = registers[registerBIndex];
    						uint operand2 = registers[registerCIndex];
    						registers[registerAIndex] = operand1 / operand2;
    						break;
    					}
    				case InstructionType.NOT_AND:
    					{
    						uint operand1 = registers[registerBIndex];
    						uint operand2 = registers[registerCIndex];
    						registers[registerAIndex] = ~(operand1 & operand2);
    						break;
    					}
    				case InstructionType.HALT:
    					{
    						halted = true;
    						Console.WriteLine("Программа остановлена");
    						break;
    					}
    				case InstructionType.ALLOCATION:
    					{
    						uint newArrayIndex =
    							memory.Allocate(registers[registerCIndex]);
    						registers[registerBIndex] = newArrayIndex;

    						break;
    					}
    				case InstructionType.ABANDONMENT:
    					{
    						memory.Abandon(registers[registerCIndex]);
    						break;
    					}
    				case InstructionType.OUTPUT:
    					{
    						Console.Write((char)registers[registerCIndex]);
    						break;
    					}
    				case InstructionType.INPUT:
    					{
    						int value = Console.Read();
    						
    						if (value > 127)
    							value = (int)'#';
    						
    						if (value == -1)
    						{
    							registers[registerCIndex] = 0xFFFFFFFF;
    						}
    						else
    							registers[registerCIndex] = (uint)value;
    						break;
    					}
    				case InstructionType.LOAD_PROGRAM:
    					{
    						memory.CopyToZeroArray(registers[registerBIndex]);
    						MoveCursorTo(registers[registerCIndex]);
    						break;
    					}
    				case InstructionType.ORTHOGRAPHY:
    					{
    						byte registerIndex = (byte)((instruction & 0x0E000000L) >> 25);
    						uint value = instruction & 0x01FFFFFF;

    						registers[registerIndex] = value;
					
  						
    						break;
    					}
    				default:
    					{
    						throw new ArgumentException("Illegal operation code " + instructionType);
    					}
    			}
			}
		}
	}
}

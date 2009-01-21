#include "processor.h"

#include <iomanip>
#include <cassert>

Processor* Processor::_instance = NULL;

Processor& Processor::get_instance() {
		if (_instance == NULL)
				_instance = new Processor();
    
		return *_instance;
}

Processor::Processor():
		_interrupted(0),
		_manager(MemoryManager::get_instance())
{
		_halted = false;
		_finger = 0;
}

void Processor::perform_program()
{
		uint instruction;
		InstructionType type;

		int a_register;
		int b_register;
		int c_register;

		while (!_halted && _interrupted != 1)
		{
				instruction = _manager[0][_finger];
				_finger++;
   
				type = InstructionType (instruction >> 28);
    
				a_register = (instruction & 0x000001c0) >> 6;
				b_register = (instruction & 0x00000038) >> 3;
				c_register = (instruction & 0x00000007);

				switch (type)
				{
				case CONDITIONAL_MOVE:
				{
						if (_registers[c_register] != 0)
								_registers[a_register] = _registers[b_register];
				
						break;
				}
				case ARRAY_INDEX:
				{
						_registers[a_register] =
								_manager[_registers[b_register]][_registers[c_register]];
				
						break;
				}
				case ARRAY_AMENDMENT:
				{
						_manager[_registers[a_register]][_registers[b_register]] =
								_registers[c_register];

						break;
				}
				case ADDITION:
				{
						_registers[a_register] =
								_registers[b_register] + _registers[c_register];

						break;
				}
				case MULTIPLICATIION:
				{
						_registers[a_register] =
								_registers[b_register] * _registers[c_register];

						break;
				}
				case DIVISION:
				{
						_registers[a_register] =
								_registers[b_register] / _registers[c_register];
                
						break;
				}
				case NOT_AND:
				{
						_registers[a_register] =
								~(_registers[b_register] & _registers[c_register]);

						break;
				}
				case HALT:
				{
						std::cout << std::endl << "Program halted." << std::endl;
						_halted = true;
          
						break;
				}
				case ALLOCATION:
				{
						_registers[b_register] = _manager.allocate(_registers[c_register]);

						break;
				}
				case ABANDONMENT:
				{
						_manager.abandon(_registers[c_register]);

						break;
				}
				case OUTPUT:
				{
						std::cout << (char)_registers[c_register];
						std::cout.flush();

						break;
				}
				case INPUT:
				{
						// Если выполнение команды прервут, то сохраняем на всякий случай
						// прежнее значение регистра.
						_register_old_value = _registers[c_register];
						unsigned char temp = std::cin.get();
         
						if (temp > 0x7f)
								temp = '*';

						if (!std::cin)
								_registers[c_register] = 0xffffffff;
						else
								_registers[c_register] = temp;
                
						break;
				}
				case LOAD_PROGRAM:
				{
						_manager.copy_array_to_zero_id(_registers[b_register]);
						_finger = _registers[c_register];

						break;
				}
				case ORPHOGRAPHY:
				{
						int a_register = (instruction << 4) >> 29;
						int value = instruction & 0x01FFFFFF;
            
						_registers[a_register] = value;
            
						break;
				}
				default:
				{
						std::cerr << "DEBUG: Incorrect instruction with code " 
											<< (int) type << std::endl;
						abort();
				}
				}
		}
}

void Processor::save_to_stream(std::ostream & stream)
{
		// Если нас прервали на состоянии ожидания ввода.
		{
				uint instruction = _manager[0][_finger - 1];
				InstructionType type = InstructionType (instruction >> 28);
				int c_register = (instruction & 0x00000007);
				
				if (type == INPUT)
				{
						--_finger;
				}
				
				_registers[c_register] = _register_old_value;
		}

		for (int i = 0; i < REGISTERS_COUNT; ++i)
		{
				write_value<uint>(stream, _registers[i]);

				DBG_CPU(std::cout << "Saved: _registers[" << i << "]=0x" 
								<< std::setw(8) << std::setfill('0') << std::hex << _registers[i] << std::endl);
		}

		write_value<uint>(stream, _halted);
		write_value<uint>(stream, _finger);

		DBG_CPU(std::cout << "Saved: _halted=0x"
						<< std::setw(8) << std::setfill('0') << std::hex << _halted << std::endl);
		DBG_CPU(std::cout << "Saved: _finger=0x" 
						<< std::setw(8) << std::setfill('0') << std::hex << _finger << std::endl);
}

void Processor::load_from_stream(std::istream & stream)
{
		for (int i = 0; i < REGISTERS_COUNT; ++i)
		{
				_registers[i] = read_value<uint>(stream);

				DBG_CPU(std::cout << "Loaded: _registers[" << i << "]=0x"
								<< std::setw(8) << std::setfill('0') << std::hex << _registers[i] << std::endl);
		}

		_halted = read_value<uint>(stream);
		_finger = read_value<uint>(stream);

		DBG_CPU(std::cout << "Loaded: _halted=0x"
						<< std::setw(8) << std::setfill('0') << std::hex << _halted << std::endl);
		DBG_CPU(std::cout << "Loaded: _finger=0x"
						<< std::setw(8) << std::setfill('0') << std::hex << _finger << std::endl);
}

void Processor::interrupt()
{
	  _interrupted = 1;
}

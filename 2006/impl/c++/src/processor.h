#ifndef PROCESSOR_H
#define PROCESSOR_H

#include <cstdlib>
#include <iostream>
#include "memorymanager.h"
#include "um.h"
#include "util.h"

#include <signal.h>

/**
    @brief Реализует состояние и систему команд процессора.

    Предоставляет методы для разбора и выполнения команд, доступ к регистрам процессора.
    
    Может загружать и сохранять себя в байтовый поток.
    
    @author Виталий Калинкин <lattyf@gmail.com>
*/
class Processor
{
public:
    static Processor& get_instance();

    void perform_program();

		void save_to_stream(std::ostream & stream);
		void load_from_stream(std::istream & stream);

		void interrupt();

		bool halted() { return _halted; }
		
private:
    static const int REGISTERS_COUNT = 8;

    static Processor* _instance;
    Processor();

    uint _registers[REGISTERS_COUNT];

    bool _halted;
    uint _finger;

		sig_atomic_t _interrupted;
		uint _register_old_value;

    MemoryManager& _manager;
};

enum InstructionType
{
    CONDITIONAL_MOVE = 0x00,
    ARRAY_INDEX      = 0x01,
    ARRAY_AMENDMENT  = 0x02,
    ADDITION         = 0x03,
    MULTIPLICATIION  = 0x04,
    DIVISION         = 0x05,
    NOT_AND          = 0x06,
    HALT             = 0x07,
    ALLOCATION       = 0x08,
    ABANDONMENT      = 0x09,
    OUTPUT           = 0x0a,
    INPUT            = 0x0b,
    LOAD_PROGRAM     = 0x0c,
    ORPHOGRAPHY      = 0x0d
};

#endif

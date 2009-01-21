#include <iostream>
#include <cstdlib>
#include <fstream>
#include <string>

#include <signal.h>
#include <sys/types.h>
#include <unistd.h>

#include "processor.h"
#include "memorymanager.h"

using namespace std;

void out_err_message()
{
    cerr << "Usage: um scroll_name"         << endl;
		cerr << "       um --load program_dump" << endl;
}

void signal_handler(int)
{
    Processor::get_instance().interrupt();
}

int main(int argc, char *argv[])
{
    //Основная программа
    Processor & processor = Processor::get_instance();
    MemoryManager & manager = MemoryManager::get_instance();

		//Регистрируем сигналы
		struct sigaction sa;
		memset(&sa, 0, sizeof(sa));
		sa.sa_handler = &signal_handler;

		sigaction(SIGINT, &sa, NULL);

    if (argc < 2)
    {
        out_err_message();
        return EXIT_FAILURE;
    }

		// Загружаем дамп
		if (strcmp(argv[1], "--load") == 0)
		{
				if (argc < 3)
				{
						out_err_message();
						return EXIT_FAILURE;
				}

				ifstream dump_file(argv[2], std::ios::binary | std::ios::in);

				if (!dump_file)
				{
						cerr << "Can't open file \"" << argv[2] << "\" for reading." << endl;
						dump_file.close();
						return EXIT_FAILURE; 
				}

				std::cout << "Loading dump file..." << std::endl;

				processor.load_from_stream(dump_file);
				manager.load_from_stream(dump_file);

				std::cout << "Dump file loaded. Starting interpretation." << std::endl;
				dump_file.close();
		}
		// Работаем с чистого образа
		else
		{

				char* scroll_name = argv[1];
        
				cout << "Reading file \"" << scroll_name << "\"." << endl;
        
				ifstream scroll_file(scroll_name, std::ios::binary | std::ios::in | std::ios::ate);
        
				if (!scroll_file)
				{
						cerr << "Can't open file \"" << scroll_name << "\" for reading." << endl;
						scroll_file.close();
						return EXIT_FAILURE; 
				}
        
				//Смотрим размер файла, чтобы выделить массив нужного объёма.
				ifstream::pos_type file_size_in_bytes = scroll_file.tellg();
				uint file_size = (static_cast<uint>(file_size_in_bytes) + sizeof(uint) - 1) / sizeof(uint);
        
				scroll_file.seekg(0, std::ios::beg);

				uint* scroll = new uint[file_size];
        
				uint temp;
				int index = 0;
				while (scroll_file)
				{
						for (int i = 0; i < 4; ++i)
						{
								temp = scroll_file.get();
                
								scroll[index] = 
										(scroll[index] << 8) | (temp & 0xff);
						}
            
						++index;
				}
        
				cout << "DEBUG: "<< (index * 4) << " bytes readed." << endl;
    
				scroll_file.close();
        
				manager.load_program_from_scroll(scroll, file_size);
        
				delete[] scroll;
		}
        
    processor.perform_program();

		if (!processor.halted())
		{
				// Нужно сохранить все данные, поскольку работа была прервана.
				std::cout << "Interrupted!" << std::endl;
				std::cout << "Dumping Universal Machine state..." << std::endl;

				std::string filename;

				std::cout << "Enter file name of dump file [um.dump]: ";

				// Если поток ввода поломали, то восстанавливаем.
				if (!std::cin)
						std::cin.clear();

				getline(std::cin, filename);
				std::cout << std::endl;

				if (filename.size() < 1)
						filename = "um.dump";

				ofstream dump_file(filename.c_str(), std::ios::binary | std::ios::out );

				processor.save_to_stream(dump_file);
				manager.save_to_stream(dump_file);

				dump_file.close();

				std::cout << "All data successfull dumped to file \"" << filename 
									<< "\". Use --load for loading this dump." << std::endl;
		}
    
    return EXIT_SUCCESS;
}

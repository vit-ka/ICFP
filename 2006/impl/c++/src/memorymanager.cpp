#include "memorymanager.h"
#include "util.h"

#include <iomanip>

MemoryManager* MemoryManager::_instance = NULL;

MemoryManager& MemoryManager::get_instance()
{
	 if (_instance == NULL)
			_instance = new MemoryManager();
   
	 return *_instance;
}

MemoryManager::MemoryManager(const MemoryManager &)
{
		
}

MemoryManager::MemoryManager()
{
	 // FIXME: Не дело так оставлять.
	 _arrays = std::vector<uint*>(20000);
	 _allocated_arrays = std::vector<bool>(20000);
	 _arrays_capasity = std::vector<uint>(20000);

	 for (int i = 0; i < 20000; ++i)
	 {
			_arrays[i] = NULL;
			_allocated_arrays[i] = false;
			_arrays_capasity[i] = 0;
	 }
    
	 //Следующий массив для выделения будет с индексом 1.
	 _next_free_array_id = 1;
}

uint * MemoryManager::operator [ ](int array_id)
{
	 return _arrays[array_id];
}

uint MemoryManager::allocate(uint capacity)
{
	 uint next_array_id = evaluate_next_free_array_id();
    
	 _arrays[next_array_id] = new uint[capacity];
	 _arrays_capasity[next_array_id] = capacity;
    
	 memset(_arrays[next_array_id], 0, capacity * sizeof(uint));
    
	 return next_array_id;
}

void MemoryManager::abandon(uint array_id)
{
	 //Главный массив удалять нельзя :)
	 assert(array_id != 0);

	 delete[] _arrays[array_id];
	 _arrays[array_id] = NULL;
	 _allocated_arrays[array_id] = false;
}

uint MemoryManager::evaluate_next_free_array_id()
{
	 uint result = _next_free_array_id;
  
	 unsigned long long size = _arrays.size();

	 //Идем первый цикл до конца.
	 while (_next_free_array_id < size
					&& _allocated_arrays[++_next_free_array_id])
			;
  
	 //Если нету свободных, то ищем сначала, вдруг там есть.
	 if (_next_free_array_id == size)			
	 {
			_next_free_array_id = 1;
				
			while (_next_free_array_id < size 
						 && _allocated_arrays[++_next_free_array_id])
				 ;
				
			//Если и второй раз ничего не нашли, значит и правда всё забили.
			if (_next_free_array_id == size )
			{
				 //Вот тут у нас кончилась память. Надо добавлять ;)
				 _arrays.resize(size + 10000);
				 _arrays_capasity.resize(size + 10000);
				 _allocated_arrays.resize(size + 10000);

				 result = _next_free_array_id++;
			}
	 }
  
	 //Все, массив теперь выделен.
	 _allocated_arrays[result] = true;
  
	 return result;
}

void MemoryManager::copy_array_to_zero_id(uint array_id)
{
	 //Самого на себя не будет копировать.
	 if (array_id == 0)
			return;

	 if (_arrays[0] != NULL)
			delete[] _arrays[0];
  
	 _arrays[0] = new uint[_arrays_capasity[array_id]];
	 _allocated_arrays[0] = true;
	 _arrays_capasity[0] = _arrays_capasity[array_id];


	 memcpy(_arrays[0],
					_arrays[array_id],
					_arrays_capasity[array_id] * sizeof(uint));
}

void MemoryManager::load_program_from_scroll(uint* scroll, uint size)
{
	 if (_arrays[0] != NULL)
			delete[] _arrays[0];
    
	 _arrays[0] = new uint[size];
    
	 memcpy(_arrays[0], scroll, size * sizeof(uint));

	 _allocated_arrays[0] = true;
	 _arrays_capasity[0] = size;
}

void MemoryManager::save_to_stream(std::ostream & stream)
{
	 write_value<uint>(stream, _next_free_array_id);
	 DBG_MEMORY(std::cout << "Saved: _next_free_array_id=0x" 
							<< std::setw(8) << std::setfill('0') << std::hex << _next_free_array_id << std::endl);

	 write_value<uint>(stream, _arrays.size());
	 DBG_MEMORY(std::cout << "Saved: _arrays.size()=0x" 
							<< std::setw(8) << std::setfill('0') << std::hex << _arrays.size() << std::endl);

	 // Запишем сначала массив _allocated_arrays.
	 for (int i = 0, size = _arrays.size(); i < size; ++i)
	 {
			write_value<bool>(stream, _allocated_arrays[i]);
	 }

	 // Теперь массив _arrays_capasity.
	 for (int i = 0, size = _arrays.size(); i < size; ++i)
	 {
			write_value<uint>(stream, _arrays_capasity[i]);
	 }

	 // И теперь уже саму память.
	 for (int i = 0, size = _arrays.size(); i < size; ++i)
	 {
			if (_allocated_arrays[i])
				 for (int j = 0, inner_array_size = _arrays_capasity[i]; j < inner_array_size; ++j)
				 {
						write_value<uint>(stream, _arrays[i][j]);
				 }
				
	 }
}

void MemoryManager::load_from_stream(std::istream & stream)
{
	 //FIXME: Удалить память из под прежних объектов.

	 _next_free_array_id = read_value<uint>(stream);
	 DBG_MEMORY(std::cout << "Loaded: _next_free_array_id=0x" 
							<< std::setw(8) << std::setfill('0') << std::hex << _next_free_array_id << std::endl);


	 uint size = read_value<uint>(stream);
	 DBG_MEMORY(std::cout << "Loaded: _arrays.size()=0x" 
							<< std::setw(8) << std::setfill('0') << std::hex << size << std::endl);
	
	 //Выделяем память под массивы.
	 _arrays = std::vector<uint*>(size, NULL);
	 _allocated_arrays = std::vector<bool>(size, false);
	 _arrays_capasity = std::vector<uint>(size, 0);

	 // Загружаем сначала массив _allocated_arrays.
	 for (uint i = 0; i < size; ++i)
	 {
			_allocated_arrays[i] = read_value<bool>(stream);
	 }

	 // Теперь массив _arrays_capasity.
	 for (uint i = 0; i < size; ++i)
	 {
			_arrays_capasity[i] = read_value<uint>(stream);
	 }

	 // И теперь уже саму память.
	 for (uint i = 0; i < size; ++i)
	 {
			if (_allocated_arrays[i])
			{
				 _arrays[i] = new uint[_arrays_capasity[i]];
				 for (int j = 0, inner_array_size = _arrays_capasity[i]; j < inner_array_size; ++j)
				 {
						_arrays[i][j] = read_value<uint>(stream);
				 }
			}
	 }
		
}

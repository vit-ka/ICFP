module um.memory;

import std.stdio;

class Memory
{
	 private:
			static Memory _instance;
		
			uint _nextFreeArrayID = 1;
			uint*[] _arrays;
			uint[] _arraysCapasity;

			this()
			{
				 _arrays = new uint*[10000];
				 _arraysCapasity = new uint[10000];
			}

			uint getFreeArrayID()
			{
				 for (uint i = _nextFreeArrayID; i < _arrays.length; ++i)
						if (_arrays[i] is null)
						{
							 _nextFreeArrayID = i + 1;
							 return i;
						}

				 // Если не нашли, проверяем, может быть раньше уже что-то освободилось.
				 for (uint i = 1; i < _nextFreeArrayID; ++i)
						if (_arrays[i] is null)
						{
							 _nextFreeArrayID = i + 1;
							 return i;
						}

				 // Память кончилась.
				 _nextFreeArrayID = _arrays.length;
				 _arrays.length = _arrays.length + 2000;
				 _arraysCapasity.length = _arraysCapasity.length + 2000;

				 return _nextFreeArrayID++;
			}

	 public:

			static Memory getInstance()
			{
				 if (_instance is null)
						_instance = new Memory();

				 return _instance;
			}

			uint allocate(uint capasity)
			{
				 if (capasity == 0)
						capasity = 1;

				 uint[] newArray = new uint[capasity];

				 uint newArrayID = getFreeArrayID();

				 // Берем адрес нулевого элемента для доступа в стиле C.
				 _arrays[newArrayID] = &newArray[0];
				 _arraysCapasity[newArrayID] = capasity;

				 return newArrayID;
			}

			void abandon(uint arrayID)
			{
				 _arrays[arrayID] = null;
				 _arraysCapasity[arrayID] = 0;
			}

			uint* data(uint arrayID)
			{
				 return _arrays[arrayID];
			}

			void copyToZeroArray(uint arrayID)
			{
				 if (arrayID == 0)
						return;

				 _arraysCapasity[0] = _arraysCapasity[arrayID];

				 uint[] tempArray = new uint[_arraysCapasity[0]];

				 for (int index = 0; index < _arraysCapasity[0]; ++index)
				 {
						tempArray[index] = _arrays[arrayID][index];
				 }

				 _arrays[0] = &tempArray[0];
			}

			void loadScroll(uint[] data)
			{
				 _arrays[0] = &data[0];
				 _arraysCapasity[0] = data.length;
			}
}
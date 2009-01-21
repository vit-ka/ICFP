#ifndef UTIL_H
#define UTIL_H

#include "um.h"
#include <iostream>

template<class T>
T read_value(std::istream & stream)
{
		char * data = new char[sizeof(T)];

		stream.read(data, sizeof(T));

		uint result = 0;

		for (uint i = 0; i < sizeof(T); ++i)
				result = result << 8 | (data[i] & 0xff);

		delete[] data;

		return result;
}

template<class T>
void write_value(std::ostream & stream, T value)
{
		char * data = new char[sizeof(T)];

		for (int i = sizeof(T) - 1; i >= 0; --i)
		{
				data[i] = value & 0xff;
				value = value >> 8;
		}

		stream.write(data, sizeof(T));

		delete[] data;
}

#endif

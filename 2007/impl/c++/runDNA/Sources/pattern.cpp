#include "pattern.h"

Endo::Pattern::Pattern()
{
	 _pattern = "";
}

void Endo::Pattern::append(std::string str)
{
	 _pattern += str;
}

std::ostream & Endo::operator<< (std::ostream & stream, Endo::Pattern & pattern)
{
	 stream << pattern._pattern;
	 return stream;
}

char Endo::Pattern::operator[](int index)
{
	return _pattern[index];
}

int Endo::Pattern::size()
{
	return _pattern.size();
}

#include "template.h"

Endo::Template::Template()
{
	 _template = "";
}

void Endo::Template::append(std::string str)
{
	 _template += str;
}

std::ostream & Endo::operator<< (std::ostream & stream, Endo::Template & pattern)
{
	 stream << pattern._template;
	 return stream;
}

char Endo::Template::operator[](int index)
{
	return _template[index];
}

int Endo::Template::size()
{
	return _template.size();
}


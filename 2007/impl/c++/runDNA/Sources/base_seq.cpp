#include "base_seq.h"

#include <iostream>
#include <istream>

Endo::BaseSeq::BaseSeq()
{
	 _seq = "";
}

Endo::BaseSeq::BaseSeq(std::istream & stream)
{
	 DBG_BASE_SEQ(
			std::cout << "Loading base sequence from file" << std::endl;
			);

	 stream >> _seq;
}

std::ostream & Endo::operator<< (std::ostream & stream, Endo::BaseSeq & dna)
{
	 stream << "First 100 bases: " << dna._seq.substr(0, 100);
	 return stream;
}

bool Endo::BaseSeq::startsWith(std::string pattern)
{
	 if (pattern.size() > _seq.size())
			return false;

	 for (unsigned int i = 0; i < pattern.size(); ++i)
			if (pattern[i] != _seq[i])
				 return false;

	 return true;
}

std::string Endo::BaseSeq::subRange(int from, int to)
{
	 return _seq.substr(from, to - from);
}

char Endo::BaseSeq::operator[](int index)
{
	return _seq[index];
}

void Endo::BaseSeq::append(std::string str)
{
	 _seq += str;
}

void Endo::BaseSeq::dropFromStart(int n)
{
// 	 DBG_BASE_SEQ(
// 			std::cout << "Dropping " << n 
// 			<< " bases from start. Current length = " 
// 			<< _seq.size() << "; New length ";
// 			);

	 _seq = _seq.substr( n );

// 	 DBG_BASE_SEQ(
// 			std::cout << _seq.size() << std::endl;
// 			);
}


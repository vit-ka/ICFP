#ifndef PATTERN_H
#define PATTERN_H

#include "config.h"
#include "base_seq.h"

#include <string>

namespace Endo
{
	 class Pattern
	 {
		 private:
			std::string _pattern;
			
		 public:
			Pattern();
			
			void append(std::string str);
			char operator[](int index);
			int size();

			friend std::ostream & operator<< (std::ostream & stream, Endo::Pattern & pattern);
	 };
	
	 std::ostream & operator<< (std::ostream & stream, Endo::Pattern & pattern);
}

#endif

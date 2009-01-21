#ifndef TEMPLATE_H
#define TEMPLATE_H

#include "config.h"
#include "base_seq.h"

#include <string>

namespace Endo
{
	 class Template
	 {
		 private:
			std::string _template;
			
		 public:
			Template();
			
			void append(std::string str);
			char operator[](int index);
			int size();

			friend std::ostream & operator<< (std::ostream & stream, Endo::Template & templ);
	 };
	
	 std::ostream & operator<< (std::ostream & stream, Endo::Template & templ);
}

#endif

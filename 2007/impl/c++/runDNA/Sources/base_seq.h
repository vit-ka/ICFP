#ifndef DNA_H
#define DNA_H

#include "config.h"

#include <string>
#include <istream>

namespace Endo
{
	 class BaseSeq
	 {
		 private:
			std::string _seq;
			
		 public:
			BaseSeq();
			BaseSeq(std::istream & stream);

			bool startsWith(std::string pattern);
			
//			void pretend(std::string & s);
			void append(std::string s);
			std::string subRange(int from, int to);
			
//		void contacenate(DNA & other_dna);
			char operator[](int index);
			void dropFromStart(int n);
//		void dropFromEnd(int n);
			int lenght();

			friend std::ostream & operator<< (std::ostream & stream, Endo::BaseSeq & seq);
	 };
	
	 std::ostream & operator<< (std::ostream & stream, Endo::BaseSeq & seq);
}

#endif

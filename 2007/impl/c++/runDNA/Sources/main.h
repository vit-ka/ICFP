#ifndef MAIN_H
#define MAIN_H

#include <iostream>

#include "config.h"
#include "base_seq.h"

void finish(Endo::BaseSeq & rna)
{
	 std::cout << "Resulting RNA: "
						 << rna << std::endl;

	 std::cout << "Program finished" << std::endl;
	 exit(0);
}

#endif

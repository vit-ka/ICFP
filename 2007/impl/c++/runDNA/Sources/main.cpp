#include <iostream>
#include <string>
#include <fstream>

using namespace std;

#include "base_seq.h"
#include "pattern.h"
#include "template.h"
#include "config.h"
#include "seq_operation.h"

int main(int argv, char ** args)
{
	 if (argv < 1)
	 {
			std::cerr << "Usage: runDNA <path-to-dna-string>" 
								<< std::endl;
	 }
	 
	 string prefix = "";
	 
	 DBG_MAIN(
			std::cout << "Starting program" << std::endl;
			);
	 
	 // Читаем поток DNA.
	 ifstream dna_stream( args[1] );
	 

	 if (!dna_stream)
	 {
			cerr << "Coudn't open " << args[1] << " file!!" << endl;
			return 1;
	 }

	 Endo::BaseSeq dna(dna_stream);
	 dna_stream.close();


	 Endo::BaseSeq rna;

	 // while (true)
	 {
			std::cout << "Starting evaluation" << std::endl;

			Endo::Pattern pattern = getPattern(dna, rna);	
			Endo::Template templ = getTemplate(dna, rna);

			DBG_MAIN(
				 std::cout << "Pattern found: " << pattern << std::endl;
				 std::cout << "Template found: " << templ << std::endl;
				 );

//			matchreplace(p, t);
	 }
}


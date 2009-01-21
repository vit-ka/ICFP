#include "seq_operation.h"
#include "main.h"

#include <vector>

#include <iostream>
#include <sstream>

int getNumber(Endo::BaseSeq & dna, Endo::BaseSeq & rna);
std::string getConsts(Endo::BaseSeq & dna);
bool isBase(char ch);

Endo::Pattern getPattern(Endo::BaseSeq & dna, Endo::BaseSeq & rna)
{
	 Endo::Pattern pattern;

	 int level = 0;

	 DBG_PATTERN (
			std::cout << "Begining pattern generation" << std::endl; );


	 DBG_PATTERN(
			std::cout << "DNA before first iteration: " << dna << std::endl;
			);
	 
	 DBG_PATTERN(
			std::cout << "Pattern before first iteration: " << pattern << std::endl;
			);
	 
	 DBG_PATTERN(
			std::cout << "RNA before first iteration: " << rna << std::endl;
			);
	 
	 
	 while (true)
	 {
			DBG_PATTERN(
				 std::cout << std::endl;
				 );

			if (dna.startsWith("C"))
			{
				 DBG_PATTERN(
						std::cout << "Found pattern \"C\"" << std::endl; );

				 dna.dropFromStart(1);
				 pattern.append("I");
			}
			else if (dna.startsWith("F"))
			{
				 DBG_PATTERN(
						std::cout << "Found pattern \"F\"" << std::endl; );

				 dna.dropFromStart(1);
				 pattern.append("C");
			}
			else if (dna.startsWith("P"))
			{
				 DBG_PATTERN(
						std::cout << "Found pattern \"P\"" << std::endl; );

				 dna.dropFromStart(1);
				 pattern.append("F");
			}
			else if (dna.startsWith("IC"))
			{
				 DBG_PATTERN(
						std::cout << "Found pattern \"IC\"" << std::endl; );

				 dna.dropFromStart(2);
				 pattern.append("P");
			}
			else if (dna.startsWith("IP"))
			{
				 DBG_PATTERN(
						std::cout << "Found pattern \"IP\"" << std::endl; );

				 dna.dropFromStart(2);

				 int number = getNumber(dna, rna);
				 pattern.append("!");

				 std::ostringstream ostr;
				 ostr << number;
				 pattern.append(ostr.str());
			}
			else if (dna.startsWith("IF"))
			{
				 DBG_PATTERN(
						std::cout << "Found pattern \"IF\"" << std::endl; );

				 dna.dropFromStart(2);

				 std::string consts = getConsts(dna);
				 pattern.append("?");
				 pattern.append(consts);
			}
			else if (dna.startsWith("IIP"))
			{
				 DBG_PATTERN(
						std::cout << "Found pattern \"IIP\"" << std::endl; );

				 dna.dropFromStart(3);

				 ++level;
				 pattern.append("(");
			}
			else if (dna.startsWith("IIC") || dna.startsWith("IIF"))
			{
				 DBG_PATTERN(
						std::cout << "Found pattern \"IIC\" or \"IIF\"" << std::endl; );

				 dna.dropFromStart(3);

				 if (level == 0)
						return pattern;
				 else
				 {
						--level;
						pattern.append(")");
				 }
			}
			else if (dna.startsWith("III"))
			{
				 DBG_PATTERN(
						std::cout << "Found data-loading pattern \"III\"" << std::endl; );

				 std::string toRNA = dna.subRange(3,10);
				 
				 rna.append(toRNA);
				 dna.dropFromStart(10);
			}
			else 
			{
				 DBG_PATTERN(
						std::cout << "Can't find known pattern. Go to finish." << std::endl; );

				 finish(rna);
			}

			DBG_PATTERN(
				 std::cout << "DNA after operation: " << dna << std::endl;
				 );

			DBG_PATTERN(
				 std::cout << "Pattern after operation: " << pattern << std::endl;
				 );

			DBG_PATTERN(
				 std::cout << "RNA after operation: " << rna << std::endl;
				 );
	 }

}

Endo::Template getTemplate(Endo::BaseSeq & dna, Endo::BaseSeq & rna)
{
	 Endo::Template templ;

	 DBG_PATTERN (
			std::cout << "Begining template generation" << std::endl; );


	 DBG_PATTERN(
			std::cout << "DNA before first iteration: " << dna << std::endl;
			);
	 
	 DBG_PATTERN(
			std::cout << "Template before first iteration: " << templ << std::endl;
			);
	 
	 DBG_PATTERN(
			std::cout << "RNA before first iteration: " << rna << std::endl;
			);
	 
	 
	 while (true)
	 {
			DBG_TEMPLATE(
				 std::cout << std::endl;
				 );

			if (dna.startsWith("C"))
			{
				 DBG_PATTERN(
						std::cout << "Found template \"C\"" << std::endl; );

				 dna.dropFromStart(1);
				 templ.append("I");
			}
			else if (dna.startsWith("F"))
			{
				 DBG_PATTERN(
						std::cout << "Found template \"F\"" << std::endl; );

				 dna.dropFromStart(1);
				 templ.append("C");
			}
			else if (dna.startsWith("P"))
			{
				 DBG_PATTERN(
						std::cout << "Found template \"P\"" << std::endl; );

				 dna.dropFromStart(1);
				 templ.append("F");
			}
			else if (dna.startsWith("IC"))
			{
				 DBG_PATTERN(
						std::cout << "Found template \"IC\"" << std::endl; );

				 dna.dropFromStart(2);
				 templ.append("P");
			}
			else if (dna.startsWith("IP") || dna.startsWith("IF"))
			{
				 DBG_PATTERN(
						std::cout << "Found template \"IP\" or \"IF\"" << std::endl; );

				 dna.dropFromStart(2);

				 int length = getNumber(dna, rna);
				 int number = getNumber(dna, rna);

				 std::ostringstream ostr;
				 ostr << "[" << number << "_" << length << "]";
				 templ.append(ostr.str());
			}
			else if (dna.startsWith("IIP"))
			{
				 DBG_PATTERN(
						std::cout << "Found template \"IIP\"" << std::endl; );

				 dna.dropFromStart(3);

				 int number = getNumber(dna, rna);

				 std::ostringstream ostr;
				 ostr << "|" << number << "|";
				 templ.append(ostr.str());
			}
			else if (dna.startsWith("IIC") || dna.startsWith("IIF"))
			{
				 DBG_PATTERN(
						std::cout << "Found template \"IIC\" or \"IIF\"" << std::endl; );

				 dna.dropFromStart(3);

				 return templ;
			}
			else if (dna.startsWith("III"))
			{
				 DBG_PATTERN(
						std::cout << "Found data-loading template \"III\"" << std::endl; );

				 std::string toRNA = dna.subRange(3,10);
				 
				 rna.append(toRNA);
				 dna.dropFromStart(10);
			}
			else 
			{
				 DBG_PATTERN(
						std::cout << "Can't find known template. Go to finish." << std::endl; );

				 finish(rna);
			}

			DBG_PATTERN(
				 std::cout << "DNA after operation: " << dna << std::endl;
				 );

			DBG_PATTERN(
				 std::cout << "Template after operation: " << templ << std::endl;
				 );

			DBG_PATTERN(
				 std::cout << "RNA after operation: " << rna << std::endl;
				 );
	 }

}

void matchAndReplace(Endo::BaseSeq & dna, Endo::Pattern & pat, Endo::Template & templ)
{
	 std::vector<Endo::BaseSeq> env;
	 std::vector<int> c;

	 for (int i = 0; i < pat.size(); ++i)
	 {
			if ( isBase(pat[i]) )
			{
				 if ( pat[i] == dna[i] )
						++i;
				 else
						return;
			}
	 }
}

bool isBase(char ch)
{
	 return ch == 'I' || ch == 'C' || ch == 'F' || ch == 'P';
}


int getNumber(Endo::BaseSeq & dna, Endo::BaseSeq & rna)
{
	 if (dna.startsWith("P"))
	 {
			dna.dropFromStart(1);
			return 0;
	 }
	 else if (dna.startsWith("I") || dna.startsWith("F"))
	 {
			dna.dropFromStart(1);

			int result = getNumber(dna, rna);
			return 2 * result;
	 }
	 else if (dna.startsWith("C"))
	 {
			dna.dropFromStart(1);

			int result = getNumber(dna, rna);
			return 2 * result + 1;
	 }
	 else
			finish(rna);

	 return 0;
}

std::string getConsts(Endo::BaseSeq & dna)
{
	 if (dna.startsWith("C"))
	 {
			dna.dropFromStart(1);
			return "I" + getConsts(dna);
	 }
	 else if (dna.startsWith("F"))
	 {
			dna.dropFromStart(1);

			return "C" + getConsts(dna);
	 }
	 else if (dna.startsWith("P"))
	 {
			dna.dropFromStart(1);

			return "F" + getConsts(dna);
	 }
	 else if (dna.startsWith("IC"))
	 {
			dna.dropFromStart(2);

			return "P" + getConsts(dna);
	 }
	 else
			return "";

}

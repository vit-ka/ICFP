#ifndef PATTERN_OPERATION_H
#define PATTERN_OPERATION_H

#include "base_seq.h"
#include "pattern.h"
#include "template.h"
#include "config.h"

Endo::Pattern	 getPattern (Endo::BaseSeq & dna, Endo::BaseSeq & rna);
Endo::Template getTemplate(Endo::BaseSeq & dna, Endo::BaseSeq & rna);
void			 matchAndReplace(Endo::BaseSeq & dna,
													 Endo::Pattern & pat,
													 Endo::Template & templ);

#endif

using System;
using System.Collections.Generic;
using System.Text;

namespace UM.Command
{
	public enum InstructionType
	{
		CONDITIONAL_MOVE	= 0x00,
		ARRAY_INDEX			= 0x01,
		ARRAY_AMENDMENT		= 0x02,
		ADDITION			= 0x03,
		MULTIPLICATION		= 0x04,
		DIVISION			= 0x05,
		NOT_AND				= 0x06,
		HALT				= 0x07,
		ALLOCATION			= 0x08,
		ABANDONMENT			= 0x09,
		OUTPUT				= 0x0a,
		INPUT				= 0x0b,
		LOAD_PROGRAM		= 0x0c,
		ORTHOGRAPHY			= 0x0d
	}
}

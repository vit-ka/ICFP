# SGrid CMakeLists.txt
# 
# To build SGrid, you will need cmake 2.4.3 (should also work with 2.4.0 and above
# but not tested). To build the application run this commands:
# 
#	cmake .
#	make
# 

PROJECT(runDNA)
CMAKE_MINIMUM_REQUIRED(VERSION 2.4.0)

# verbose - off
#SET( CMAKE_VERBOSE_MAKEFILE  ON )

SET( runDNA_SRCS
		 Sources/main.cpp
		 Sources/base_seq.cpp
		 Sources/seq_operation.cpp
		 Sources/pattern.cpp
		 Sources/template.cpp
	)

# definitions for the project
ADD_DEFINITIONS(
	-Wall -ggdb
	)

# include directories
INCLUDE_DIRECTORIES( 
	${CMAKE_BINARY_DIR}
	.
	Sources 
	)

# the main build target
ADD_EXECUTABLE(runDNA
	${runDNA_SRCS} 
	)


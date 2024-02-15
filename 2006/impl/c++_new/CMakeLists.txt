cmake_minimum_required(VERSION 3.7)

set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)


project(UniversalMachine CXX)
set(CMAKE_BUILD_TYPE Debug)

add_executable(um main.cpp format.cc)


# Debug
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -O2 -std=c++2a -fsanitize=address -Werror -pthread")

# Release
# set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -O2 -std=c++2a -Werror -pthread")

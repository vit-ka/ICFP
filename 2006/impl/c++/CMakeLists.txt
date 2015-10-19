cmake_minimum_required(VERSION 3.3)
project(um CXX)
add_executable(um src/memorymanager.cpp src/processor.cpp src/um.cpp)
target_compile_features(um PRIVATE cxx_auto_type)

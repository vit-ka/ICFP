cmake_minimum_required(VERSION 3.3)
project(um CXX)
set(CMAKE_BUILD_TYPE Debug)
add_executable(um src/main.cpp)
target_compile_features(um PRIVATE cxx_auto_type)


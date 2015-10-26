cmake_minimum_required(VERSION 3.3)

project(um CXX)

list(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_LIST_DIR}/cmake")

set(CMAKE_BUILD_TYPE Debug)
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

find_package(gflags REQUIRED)
find_package(glog REQUIRED)

add_executable(um src/main.cpp src/ScrollLoader.cpp src/UM.cpp)
target_link_libraries(um gflags glog)
target_compile_features(um PRIVATE cxx_auto_type)

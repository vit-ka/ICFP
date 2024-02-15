#include <array>
#include <fstream>
#include <iostream>
#include <string>
#include <vector>

#include "fmt/core.h"

std::vector<uint32_t> readScroll(const std::string &file) {
  std::vector<uint32_t> result;
  std::ifstream program(file);
  if (program) {
    program.seekg(0, program.end);
    size_t len = program.tellg();
    program.seekg(0, program.beg);

    result.resize(len);
    program.read(reinterpret_cast<char *>(result.data()), len);

    if (program) {
      std::cerr << "\tRead " << len << " bytes of scroll " << file << std::endl;
    } else {
      std::cerr << "\terror: could only read " << program.gcount()
                << " bytes of scroll " << file << std::endl;
    }

    program.close();
  }
  return result;
}

int main(int argc, char *argv[]) {
  std::array<uint32_t, 9> rs{};
  std::vector<std::vector<uint32_t>> mem;
  size_t ip = 0;

  if (argc < 2) {
    std::cout << "\tUsage: um <image>" << std::endl;
    return -1;
  }

  mem.resize(1);
  mem[0] = readScroll(argv[1]);

  while (true) {
    auto platter = mem[0][ip];
    auto op = (platter & 0xf0000000) >> 27;
    size_t a, b, c;
    a = platter & 0x7;
    b = (platter & 0x38) >> 3;
    c = (platter & 0x1c0) >> 6;

    std::cerr << "[" << fmt::format("{:0x}", ip) << "] Read op #" << op << "("
              << a << ";" << b << ";" << c << ")" << std::endl;
  }

  return 0;
}

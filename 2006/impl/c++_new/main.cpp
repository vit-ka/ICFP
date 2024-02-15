#include <array>
#include <fstream>
#include <iostream>
#include <string>
#include <vector>

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

  if (argc < 1) {
    std::cout << "\tUsage: um <image>" << std::endl;
    return -1;
  }

  mem.resize(1);
  mem[0] = readScroll(argv[1]);

  return 0;
}

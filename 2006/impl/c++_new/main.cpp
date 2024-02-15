#include <array>
#include <fstream>
#include <iostream>
#include <string>
#include <vector>

#include "fmt/core.h"

enum Op {
  ConditionaMove = 0x0,
  Addition = 0x3,
  Halt = 0x7,
  Output = 0xa,
  LoadProgram = 0xc,
  Orthography = 0xd
};

std::vector<uint32_t> readScroll(const std::string &file) {
  std::vector<uint32_t> result;
  std::ifstream program(file);
  if (program) {
    program.seekg(0, program.end);
    size_t len = program.tellg();
    program.seekg(0, program.beg);

    result.resize(len / sizeof(uint32_t));
    program.read(reinterpret_cast<char *>(result.data()), len);

    // Swapping endiness
    for (auto &v : result) {
      uint8_t *pV = reinterpret_cast<uint8_t *>(&v);
      std::reverse(pV, pV + sizeof(v));
    }

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
    auto op = static_cast<Op>((platter & 0xf0000000) >> 28);
    size_t a, b, c;
    a = (platter & 0x1c0) >> 6;
    b = (platter & 0x38) >> 3;
    c = platter & 0x7;

    if (op != Op::Orthography) {
      std::cerr << "[ip:" << fmt::format("0x{:08x}", ip)
                << "][pl:" << fmt::format("0x{:08x}", platter) << "] opcode #"
                << op << "(" << a << ";" << b << ";" << c << ")" << std::endl;
    }
    switch (op) {
    case ConditionaMove: {
      if (rs[c]) {
        rs[a] = rs[b];
      }
      ++ip;
      break;
    }
    case Addition: {
      rs[a] = rs[b] + rs[c];
      ++ip;
      break;
    }
    case Halt: {
      std::cerr << "\tHalted" << std::endl;
      return -1;
    }
    case Output: {
      std::cout << static_cast<char>(rs[c]);
      ++ip;
      break;
    }
    case LoadProgram: {
      if (rs[b] >= mem.size()) {
        std::cerr << "\terror: Array " << rs[b]
                  << " is outside allocated area. Max array #: "
                  << mem.size() - 1 << std::endl;
      }
      if (rs[b]) {
        mem[0] = mem[rs[b]];
      }
      ip = rs[c];
      break;
    }
    case Orthography: {
      a = (platter & 0xe000000) >> 25;
      uint32_t value = platter & 0x1ffffff;
      std::cerr << "[ip:" << fmt::format("0x{:08x}", ip)
                << "][pl:" << fmt::format("0x{:08x}", platter) << "] opcode #"
                << op << "(" << a
                << "). Value: " << fmt::format("0x{:08x}", value) << std::endl;
      rs[a] = value;
      ++ip;
      break;
    }
    default:
      std::cerr << "Unknown opcode: [ip:" << fmt::format("0x{:08x}", ip) << "]["
                << fmt::format("0x{:08x}", platter) << "] opcode #" << op << "("
                << a << ";" << b << ";" << c << ")" << std::endl;
      return -1;
    }
    std::cerr << "\trs[";
    for (const auto &r : rs) {
      std::cerr << fmt::format("0x{:02x};", r);
    }
    std::cerr << "]" << std::endl;
  }

  return 0;
}

#include <array>
#include <fstream>
#include <iostream>
#include <string>
#include <vector>

#include "fmt/core.h"

#define DBG_CPU(x) ; // DBG_CPU(x) x;
#define DBG_RS(x) ;

enum Op {
  ConditionaMove = 0x0,
  ArrayIndex = 0x1,
  ArrayAmendment = 0x2,
  Addition = 0x3,
  Multiplication = 0x4,
  Division = 0x5,
  NotAnd = 0x6,
  Halt = 0x7,
  Allocation = 0x8,
  Abandonment = 0x9,
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
  std::array<uint32_t, 8> rs{};
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
      DBG_CPU(std::cerr << "[ip:" << fmt::format("0x{:08x}", ip)
                        << "][pl:" << fmt::format("0x{:08x}", platter)
                        << "] opcode #" << op << "(" << a << ";" << b << ";"
                        << c << ")" << std::endl);
    }
    switch (op) {
    case ConditionaMove: {
      if (rs[c]) {
        rs[a] = rs[b];
      }
      ++ip;
      break;
    }
    case ArrayIndex: {
      if (rs[b] >= mem.size()) {
        std::cerr << "\terror: Array Index " << rs[b]
                  << " is outside allocated area. Max array #: "
                  << mem.size() - 1 << std::endl;
      }

      if (rs[c] >= mem[rs[b]].size()) {
        std::cerr << "\terror: Offset " << rs[c]
                  << " is outside allocated area in array # " << rs[b]
                  << ". Max #: " << mem[rs[b]].size() - 1 << std::endl;
      }
      rs[a] = mem[rs[b]][rs[c]];
      ++ip;
      break;
    }
    case ArrayAmendment: {
      if (rs[a] >= mem.size()) {
        std::cerr << "\terror: Array Index " << rs[a]
                  << " is outside allocated area. Max array #: "
                  << mem.size() - 1 << std::endl;
      }

      if (rs[b] >= mem[rs[a]].size()) {
        std::cerr << "\terror: Offset " << rs[b]
                  << " is outside allocated area in array # " << rs[a]
                  << ". Max #: " << mem[rs[a]].size() - 1 << std::endl;
      }
      mem[rs[a]][rs[b]] = rs[c];
      ++ip;
      break;
    }
    case Addition: {
      rs[a] = rs[b] + rs[c];
      ++ip;
      break;
    }
    case Multiplication: {
      rs[a] = rs[b] * rs[c];
      ++ip;
      break;
    }
    case Division: {
      rs[a] = rs[b] / rs[c];
      ++ip;
      break;
    }
    case NotAnd: {
      rs[a] = ~(rs[b] & rs[c]);
      ++ip;
      break;
    }
    case Halt: {
      std::cerr << "\tHalted" << std::endl;
      return -1;
    }
    case Allocation: {
      mem.push_back(std::vector<uint32_t>(rs[c], 0));
      rs[b] = mem.size() - 1;
      ++ip;
      break;
    }
    case Abandonment: {
      if (rs[c] >= mem.size()) {
        std::cerr << "\terror: Array Index " << rs[c]
                  << " is outside allocated area. Max array #: "
                  << mem.size() - 1 << std::endl;
      }
      mem[rs[c]] = {};
      ++ip;
      break;
    }
    case Output: {
      std::cout << static_cast<char>(rs[c]);
      ++ip;
      break;
    }
    case LoadProgram: {
      if (rs[b] >= mem.size()) {
        std::cerr << "\terror: Array Index " << rs[b]
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
      DBG_CPU(std::cerr << "[ip:" << fmt::format("0x{:08x}", ip)
                        << "][pl:" << fmt::format("0x{:08x}", platter)
                        << "] opcode #" << op << "(" << a << "). Value: "
                        << fmt::format("0x{:08x}", value) << std::endl);
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

    DBG_RS(std::cerr << "\trs[");
    DBG_RS(for (auto r : rs) { std::cerr << fmt::format("0x{:02x};", r); })
    DBG_RS(std::cerr << "\b]" << std::endl);
  }

  return 0;
}

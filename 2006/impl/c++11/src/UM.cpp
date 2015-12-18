#include <type_traits>
#include <iostream>
#include <istream>

#include <glog/logging.h>

#include "UM.h"

using namespace std;

std::ostream& operator<< (std::ostream& out, const CpuInstruction& instr);

UM::UM(vector<uint32_t>&& scroll)
  : memory_(move(scroll)),
    idx_(0),
    regs_{0, 0, 0, 0, 0, 0, 0, 0},
    halt_(false) {

  LOG(INFO) << "Received scroll has been put into memory " << memory_;
}

void UM::executeAllSteps() {
  LOG(INFO) << "=======================================================";
  LOG(INFO) << "           Starting scroll execution...";
  LOG(INFO) << "=======================================================";

  while (idx_ < memory_[0].size() && !halt_) {
    auto raw_instr = memory_[0][idx_];
    CpuInstruction instruction(raw_instr);

    ++idx_;

    executeInstruction(instruction);
  }
}

void UM::executeInstruction(const CpuInstruction& instr) {
  VLOG(15) << "Executing instruction at position " << idx_ << ": " << instr;

  // Warning! Gavnocode!
  switch (instr.type()) {
    case CpuInstructionType::CONDITIONAL_MOVE:
      if (regs_[instr.regC()] != 0) {
        regs_[instr.regA()] = regs_[instr.regB()];
      }
      break;
    case CpuInstructionType::ARRAY_INDEX:
      regs_[instr.regA()] = memory_[regs_[instr.regB()]][regs_[instr.regC()]];
      break;
    case CpuInstructionType::ARRAY_AMENDMENT:
      memory_[regs_[instr.regA()]][regs_[instr.regB()]] = regs_[instr.regC()];
      break;
    case CpuInstructionType::ADDITION:
      regs_[instr.regA()] = regs_[instr.regB()] + regs_[instr.regC()];
      break;
    case CpuInstructionType::MULTIPLICATION:
      regs_[instr.regA()] = regs_[instr.regB()] * regs_[instr.regC()];
      break;
    case CpuInstructionType::DIVISION:
      regs_[instr.regA()] = regs_[instr.regB()] / regs_[instr.regC()];
      break;
    case CpuInstructionType::NOT_AND:
      regs_[instr.regA()] = ~(regs_[instr.regB()] & regs_[instr.regC()]);
      break;
    case CpuInstructionType::HALT:
      LOG(WARNING) << "Program halted!";
      halt_ = true;
      break;
    case CpuInstructionType::ALLOCATION:
      regs_[instr.regB()] = memory_.allocate(regs_[instr.regC()]);
      break;
    case CpuInstructionType::ABANDONMENT:
      memory_.abandon(regs_[instr.regC()]);
      break;
    case CpuInstructionType::OUTPUT:
      std::cout << static_cast<char>(regs_[instr.regC()]);
      std::cout.flush();
      break;
    case CpuInstructionType::INTPUT:
      {
        unsigned char ch = std::cin.get();

        if (ch > 0x7f) {
          ch = '*';
        }

        if (std::cin /* ^C */) {
          regs_[instr.regC()] = ch;
        } else {
          regs_[instr.regC()] = 0xFFFFFFFF;
        }
        break;
      }
    case CpuInstructionType::LOAD_PROGRAM:
      memory_.loadToZero(regs_[instr.regB()]);
      idx_ = regs_[instr.regC()];
      break;
    case CpuInstructionType::ORPHOGRAPY:
      regs_[instr.regA()] = instr.orphographyValue();
      break;
    default:
      LOG(WARNING) << "Unsupported operation: " << instr;
  }
}

CpuInstruction::CpuInstruction(uint32_t raw_instr) {
  type_ = static_cast<CpuInstructionType>((raw_instr >> 28) & 0x0F);

  if (CpuInstructionType::ORPHOGRAPY == type_) {
    regA_ = (raw_instr >> 25) & 0x00000007;
    regB_ = 0;
    regC_ = 0;
    orphographyValue_ = raw_instr & 0x01FFFFFF;
  } else {
    regA_ = (raw_instr & 0x000001C0) >> 6;
    regB_ = (raw_instr & 0x00000038) >> 3;
    regC_ =  raw_instr & 0x00000007;
    orphographyValue_ = 0;
  }
}

std::ostream& operator<< (std::ostream& out, const CpuInstructionType& value) {
  out << +static_cast<std::underlying_type<CpuInstructionType>::type>(value);
  return out;
}

std::ostream& operator<< (std::ostream& out, const CpuInstruction& instr) {
  out << hex 
      << "0x" << instr.type()
      << "[A=0x" << +instr.regA();

  if (CpuInstructionType::ORPHOGRAPY == instr.type()) {
    out << ",value=0x" << +instr.orphographyValue();
  } else {
    out << ",B=0x" << +instr.regB() << ",C=0x" << +instr.regC();
  }

  out << "]" << dec;

  return out;
}

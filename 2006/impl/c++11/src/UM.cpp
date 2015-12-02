#include <glog/logging.h>

#include "UM.h"

using namespace std;

std::ostream& operator<< (std::ostream& out, const CpuInstruction& instr);

UM::UM(vector<uint32_t>&& scroll)
  : memory_(move(scroll)), idx_(0), regs_({0, 0, 0, 0, 0, 0, 0, 0}) {

  LOG(INFO) << "Received scroll has been put into memory " << memory_;
}

void UM::ExecuteAllSteps() {
  LOG(INFO) << "Starting scroll execution...";

  while (idx_ < memory_[0].size()) {
    auto raw_instr = memory_[0][idx_];
    CpuInstruction instruction(raw_instr);

    LOG(INFO) << "Decoded instruction at position " << idx_ << ": " << instruction;

    ++idx_;
  }
}

CpuInstruction::CpuInstruction(uint32_t raw_instr) {
  type_ = static_cast<CpuInstructionType>((raw_instr >> 28) & 0x0F);

  if (CpuInstructionType::ORPHOGRAPY == type_) {
    regA_ = 0;
    regB_ = 0;
    regC_ = 0;
    orphographyValue_ = 0;
  } else {
    regA_ = raw_instr & 0x07;
    regB_ = (raw_instr & 0x38) >> 3;
    regC_ = (raw_instr & 0x01C0) >> 6;
  }
}

std::ostream& operator<< (std::ostream& out, const CpuInstruction& instr) {
  out << instr.type()
      << hex << "[A=" << instr.regA() << ",B=" << instr.regB() << ",C" << instr.regC()
      << ",value=" << instr.orphographyValue() << "]" << dec;
  return out;
}

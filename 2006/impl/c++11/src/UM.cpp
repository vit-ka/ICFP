#include <type_traits>

#include <glog/logging.h>

#include "UM.h"

using namespace std;

std::ostream& operator<< (std::ostream& out, const CpuInstruction& instr);

UM::UM(vector<uint32_t>&& scroll)
  : memory_(move(scroll)), idx_(0), regs_({0, 0, 0, 0, 0, 0, 0, 0}) {

  LOG(INFO) << "Received scroll has been put into memory " << memory_;
}

void UM::ExecuteAllSteps() {
  LOG(INFO) << "=======================================================";
  LOG(INFO) << "           Starting scroll execution...";
  LOG(INFO) << "=======================================================";

  //while (idx_ < memory_[0].size()) {
  while (idx_ < 10) {
    auto raw_instr = memory_[0][idx_];
    CpuInstruction instruction(raw_instr);

    LOG(INFO) << "Decoded instruction at position " << idx_ << ": " << instruction;

    ++idx_;
  }
}

CpuInstruction::CpuInstruction(uint32_t raw_instr) {
  type_ = static_cast<CpuInstructionType>((raw_instr >> 28) & 0x0F);

  if (CpuInstructionType::ORPHOGRAPY == type_) {
    regA_ = (raw_instr >> 26) & 0x07;
    regB_ = 0;
    regC_ = 0;
    orphographyValue_ = raw_instr & 0x03FFFFFF;
  } else {
    regA_ = raw_instr & 0x07;
    regB_ = (raw_instr & 0x38) >> 3;
    regC_ = (raw_instr & 0x01C0) >> 6;
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

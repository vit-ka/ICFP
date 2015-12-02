#pragma once

#include <array>

#include "MemoryArray.h"

class UM {
  public:
    UM(std::vector<uint32_t>&& scroll);
    void ExecuteAllSteps();

  private:
    MemoryArray memory_;
    uint32_t idx_;
    std::array<uint32_t, 8> regs_;
};

enum CpuInstructionType: uint8_t {
  CONDITIONAL_MOVE = 0x00,
  ARRAY_INDEX      = 0x01,
  ORPHOGRAPY       = 0x0D,
};

class CpuInstruction {
  public:
    CpuInstruction(uint32_t raw_instr);
    CpuInstructionType type() const { return type_; }
    uint8_t regA() const { return regA_; }
    uint8_t regB() const { return regB_; }
    uint8_t regC() const { return regC_; }
    uint32_t orphographyValue() const { return orphographyValue_; }

  private:
    CpuInstructionType type_;
    uint8_t regA_;
    uint8_t regB_;
    uint8_t regC_;
    uint32_t orphographyValue_;
};

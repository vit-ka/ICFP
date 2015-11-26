#pragma once

#include "MemoryArray.h"

class UM {
  public:
    UM(std::vector<uint32_t>&& scroll);
    void ExecuteAllSteps();

  private:
    MemoryArray memory;
};

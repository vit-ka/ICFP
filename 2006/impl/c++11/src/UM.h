#pragma once

class UM {
  public:
    UM(std::vector<uint32_t>&& scroll);
    void ExecuteAllSteps();
};

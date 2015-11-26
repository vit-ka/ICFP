#pragma once

class MemoryArray {
  public:
    MemoryArray(std::vector<uint32_t>&& scroll);
    friend std::ostream& operator<< (std::ostream& stream, const MemoryArray& memory);

  private:
    std::vector<std::vector<uint32_t>> plates;
};


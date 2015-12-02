#pragma once

class MemoryArray {
  public:
    MemoryArray(std::vector<uint32_t>&& scroll);
    std::vector<uint32_t>& operator[](std::size_t idx) { return plates[idx]; }

    friend std::ostream& operator<< (std::ostream& stream, const MemoryArray& memory);

  private:
    std::vector<std::vector<uint32_t>> plates;
};


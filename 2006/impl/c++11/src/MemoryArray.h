#pragma once

class MemoryArray {
  public:
    MemoryArray(std::vector<uint32_t>&& scroll);
    uint32_t allocate(size_t size);
    void abandon(uint32_t index);
    void loadToZero(uint32_t index);

    std::vector<uint32_t>& operator[](std::size_t idx) { return *plates_[idx]; }

    friend std::ostream& operator<< (std::ostream& stream, const MemoryArray& memory);

  private:
    std::vector<std::unique_ptr<std::vector<uint32_t>>> plates_;
};


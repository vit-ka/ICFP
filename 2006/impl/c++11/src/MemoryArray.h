#pragma once

class MemoryArray {
  public:
    explicit MemoryArray(std::vector<uint32_t>&& scroll);
    explicit MemoryArray(std::vector<uint8_t>&& dump);
    uint32_t allocate(size_t size);
    void abandon(uint32_t index);
    void loadToZero(uint32_t index);

    // Non-const to allow assignment to internals of the vector.
    std::vector<uint32_t>& operator[](std::size_t idx) { return *plates_[idx]; }

    const std::vector<uint8_t> serialize();

    friend std::ostream& operator<< (std::ostream& stream, const MemoryArray& memory);

  private:
    std::vector<std::unique_ptr<std::vector<uint32_t>>> plates_;
};

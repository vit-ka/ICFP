#include <glog/logging.h>

#include "MemoryArray.h"

using namespace std;

MemoryArray::MemoryArray(vector<uint32_t>&& scroll) :
  plates_() {
  LOG(INFO) << "Received new scroll of size " << scroll.size() << " commands. Hiding away.";

  plates_.resize(1);
  std::unique_ptr<std::vector<uint32_t>> local_scroll_ptr(
      new std::vector<uint32_t>(std::move(scroll)));
  plates_[0] = std::move(local_scroll_ptr);

  LOG(INFO) << "Scroll has been put to plate 0.";
}

uint32_t MemoryArray::allocate(size_t size) {
  std::unique_ptr<std::vector<uint32_t>> new_array_ptr(
      new std::vector<uint32_t>(size));
  uint32_t new_index = plates_.size();
  plates_.push_back(std::move(new_array_ptr));
  VLOG(10) << "Allocated new array at " << new_index
    << " with size " << size;
  return new_index;
}

void MemoryArray::abandon(uint32_t index) {
  VLOG(10) << "Abandoning array " << index;
  plates_[index].reset();
}

void MemoryArray::loadToZero(uint32_t index) {
  if (index == 0)
    return;

  auto copy = std::unique_ptr<std::vector<uint32_t>>(
      new std::vector<uint32_t>(*plates_[index]));
  plates_[0].reset();
  plates_[0] = std::move(copy);

  VLOG(10) << "Array at index " << index
    << " has been placed instead of array 0";
}

std::ostream& operator<< (std::ostream& out, const MemoryArray& memory) {
  out << "MEMORY: [" << endl;
  for (const auto & plate : memory.plates_) {
    out << "\t[" << plate->size() << " commands]" << endl;
  }
  out << "]" << endl;
  return out;
}

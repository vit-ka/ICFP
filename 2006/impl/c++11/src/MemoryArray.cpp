#include <glog/logging.h>

#include "MemoryArray.h"

using namespace std;

MemoryArray::MemoryArray(vector<uint32_t>&& scroll) :
  plates_() {
  LOG(INFO) << "Received new scroll of size " << scroll.size() << " commands. Hiding away.";

  plates_.resize(1);
  plates_[0] = move(scroll);

  LOG(INFO) << "Scroll has been put to plate 0.";
}

std::ostream& operator<< (std::ostream& out, const MemoryArray& memory) {
  out << "MEMORY: [" << endl;
  for (const auto & plate : memory.plates_) {
    out << "\t[" << plate.size() << " commands]" << endl;
  }
  out << "]" << endl;
  return out;
}

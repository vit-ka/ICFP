#include <glog/logging.h>

#include "MemoryArray.h"

using namespace std;

MemoryArray::MemoryArray(vector<uint32_t>&& scroll) {
  LOG(INFO) << "Received new scroll of size " << scroll.size() << " commands. Hiding away...";
}

#include <glog/logging.h>

#include "UM.h"

using namespace std;

UM::UM(vector<uint32_t>&& scroll)
  : memory(move(scroll)) {

  LOG(INFO) << "Received scroll of " << scroll.size()
      << " commands converted to memory array...";
}

void UM::ExecuteAllSteps() {
  LOG(INFO) << "Not implemented.";
}

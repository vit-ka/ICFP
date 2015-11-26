#include <glog/logging.h>

#include "UM.h"

UM::UM(std::vector<uint32_t>&& scroll) {
  LOG(INFO) << "Received scroll of " << (scroll.size()) 
      << " commands. Converting to memory array...";
}

void UM::ExecuteAllSteps() {
  LOG(INFO) << "Not implemented.";
}

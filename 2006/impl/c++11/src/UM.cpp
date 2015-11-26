#include <glog/logging.h>

#include "UM.h"

using namespace std;

UM::UM(vector<uint32_t>&& scroll)
  : memory(move(scroll)) {

  LOG(INFO) << "Received scroll has been put into memory " << memory;
}

void UM::ExecuteAllSteps() {
  LOG(INFO) << "ExecuteAllSteps: Not implemented.";
}

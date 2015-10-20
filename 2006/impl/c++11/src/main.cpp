#include <iostream>

#include <gflags/gflags.h>
#include <glog/logging.h>

DEFINE_string(load, "", "Dump file name to load.");

static const char* VERSION = "0.0.1";
static const char* USAGE_MESSAGE = "[flags] scroll_file.um";

using namespace std;

int main(int argc, char* argv[]) {
  google::SetVersionString(VERSION);
  google::SetUsageMessage(USAGE_MESSAGE);
  google::ParseCommandLineFlags(&argc, &argv, true);
  google::InitGoogleLogging(argv[0]);

  if (FLAGS_load != "") {
    LOG(INFO) << "Dump file loading mode. I will load '" << FLAGS_load
              << "' file and continue execution from the previously saved state.";
  } else {
    if (argc < 2) {
      LOG(FATAL) << "The scroll file name should be specified as the first argument.";
    }

    LOG(INFO) << "Normal startup mode. Image file name to execute: '" << argv[1] << "'";
  }
}

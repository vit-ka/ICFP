#include <iostream>

#include <gflags/gflags.h>
#include <glog/logging.h>

#include "ScrollLoader.h"

DEFINE_string(load, "", "Dump file name to load.");

static const char* VERSION = "0.0.1";
static const char* USAGE_MESSAGE = "[flags] scroll_file.um";

using namespace std;

void LoadDumpFile(const string& file_name);
void LoadRegularScrollAddExecuteAllSteps(const string& file_name);

int main(int argc, char* argv[]) {
  google::SetVersionString(VERSION);
  google::SetUsageMessage(USAGE_MESSAGE);
  google::ParseCommandLineFlags(&argc, &argv, true);
  google::InitGoogleLogging(argv[0]);
  google::InstallFailureSignalHandler();

  if (FLAGS_load != "") {
    if (argc > 1) {
      LOG(FATAL) << "You should've not specified any regular scroll name while loading "
                 << "from a dump file.";
    }

    LOG(INFO) << "Dump file loading mode. I will load '" << FLAGS_load
              << "' file and continue execution from the previously saved state.";

    LoadDumpFile(FLAGS_load);
  } else {
    if (argc < 2) {
      LOG(FATAL) << "The scroll file name should be specified as the first argument.";
    }

    LOG(INFO) << "Normal startup mode. Image file name to execute: '" << argv[1] << "'";
    LoadRegularScrollAddExecuteAllSteps(string(argv[1]));
  }
}

void LoadDumpFile(const string& file_name) {
  VLOG(1) << "Loading dump file '" << file_name << "'...";
  LOG(FATAL) << "This functions has not been implemented yet";
}

void LoadRegularScrollAndExecuteAllSteps(const string& file_name) {
  VLOG(1) << "Loading regular scroll file '" << file_name << "'...";

  ScrollLoader loader;
  auto um = loader.PrepareUmFromScrollFile(file_name);

  um.ExecuteAllSteps();

  LOG(INFO) << "Regular scroll file execution has finished. File name: '"
            << file_name << "'.";
}


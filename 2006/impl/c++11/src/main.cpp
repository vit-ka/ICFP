#include <iostream>

#include <gflags/gflags.h>

DEFINE_string(load, "", "Dump file name to load.");

static const char* VERSION = "0.0.1";
static const char* USAGE_MESSAGE = "[flags] scroll_file.um";

using namespace std;

int main(int argc, char* argv[]) {
  google::SetVersionString(VERSION);
  google::SetUsageMessage(USAGE_MESSAGE);
  google::ParseCommandLineFlags(&argc, &argv, true);

  if (FLAGS_load != "") {
    cout << "Dump file loading mode. I will load '" << FLAGS_load 
         << "' file and continue execution from the previously saved state." << endl;
  } else {
    cout << "Normal startup mode. Image file name to execute: '" << argv[1] << "'" << endl;
  }
}

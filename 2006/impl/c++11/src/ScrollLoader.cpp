#include <glog/logging.h>
#include <fstream>

#include "ScrollLoader.h"

using namespace std;

UM ScrollLoader::PrepareUmFromScrollFile(const string& file_name) {
  ifstream in_stream(file_name);
  LOG(INFO) << "Loading file '" << file_name << "'...";

  unique_ptr<char[]> buffer(new char[4]);
  unique_ptr<vector<uint32_t> > scroll(new vector<uint32_t>());

  while (in_stream) {
    char * buffer_ptr = buffer.get();
    in_stream.read(buffer_ptr, 4);

    uint32_t block = buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24;

    scroll->push_back(block);
  }

  LOG(INFO) << "Read " << (scroll->size() * 4) << " bytes";

  UM result;

  return result;
}


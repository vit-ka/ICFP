#include <glog/logging.h>
#include <fstream>
#include <array>

#include "ScrollLoader.h"

using namespace std;

const uint32_t BUFFER_SIZE = 10240;

UM ScrollLoader::PrepareUmFromScrollFile(const string& file_name) {
  ifstream in_stream(file_name);
  LOG(INFO) << "Loading file '" << file_name << "'...";

  array<char, BUFFER_SIZE> buffer;
  vector<uint32_t> scroll;

  while (in_stream) {
    in_stream.read(buffer.data(), BUFFER_SIZE);
    auto read_bytes = in_stream.gcount();

    for (auto i = 0; i < read_bytes; i += 4) {
      uint32_t block = buffer[i] | buffer[i + 1] << 8 
          | buffer[i + 2] << 16 | buffer[i + 3] << 24;
      scroll.push_back(block);
    }
  }

  LOG(INFO) << "Read " << (scroll.size() * 4) << " bytes";
  in_stream.close();

  UM result(std::move(scroll));

  return result;
}


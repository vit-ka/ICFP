#include <glog/logging.h>
#include <fstream>
#include <array>

#include "ScrollLoader.h"

using namespace std;

const uint32_t BUFFER_SIZE = 10240;

UM ScrollLoader::prepareUmFromScrollFile(const string& file_name) {
  ifstream in_stream(file_name);
  LOG(INFO) << "Loading file '" << file_name << "'...";

  array<char, BUFFER_SIZE> buffer;
  vector<uint32_t> scroll;

  while (in_stream) {
    in_stream.read(buffer.data(), BUFFER_SIZE);
    auto read_bytes = in_stream.gcount();

    for (auto i = 0; i < read_bytes; i += 4) {
      uint32_t block =
          static_cast<uint32_t>(0xFF & buffer[i]) << 24
        | static_cast<uint32_t>(0xFF & buffer[i + 1]) << 16
        | static_cast<uint32_t>(0xFF & buffer[i + 2]) << 8
        | static_cast<uint32_t>(0xFF & buffer[i + 3]);
      scroll.push_back(block);
    }
  }

  LOG(INFO) << "Read " << (scroll.size() * 4) << " bytes";
  in_stream.close();

  UM result(std::move(scroll));

  return result;
}


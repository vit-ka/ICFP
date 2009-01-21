TEMPLATE = app
TARGET = 
DEPENDPATH += . src
INCLUDEPATH += . src

# Input
HEADERS += src/memorymanager.h \
	src/processor.h \
	src/um.h
SOURCES += src/memorymanager.cpp \
	src/processor.cpp \
	src/um.cpp \
  src/util.cpp

DESTDIR +=       ./Dist
OBJECTS_DIR +=   ./Dist/obj

CONFIG = release

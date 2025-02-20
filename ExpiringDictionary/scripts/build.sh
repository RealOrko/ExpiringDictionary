#!/bin/bash

mkdir -p build && cd build
cmake --build . --config Release
cd ..

mkdir -p ../ExpiringDictionaryWrapper/src/lib
cp build/libExpiringDictionary.so ../ExpiringDictionaryWrapper/src/lib

mkdir -p ../ExpiringDictionaryWrapper/tests/lib
cp build/libExpiringDictionary.so ../ExpiringDictionaryWrapper/tests/lib

cmake_minimum_required(VERSION 3.15)
project(ExpiringDictionary VERSION 1.0.0 LANGUAGES CXX)

# Set C++ standard
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Define the shared library
add_library(ExpiringDictionary SHARED src/ExpiringDictionary.cpp)
target_include_directories(ExpiringDictionary PUBLIC
    $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}/include>
    $<INSTALL_INTERFACE:include>
)

# Add test executable (optional)
add_executable(ExpiringDictionaryTest test/main.cpp)
target_link_libraries(ExpiringDictionaryTest PRIVATE ExpiringDictionary)

# Enable testing
enable_testing()
add_test(NAME DictionaryTest COMMAND ExpiringDictionaryTest)

# Installation rules
install(TARGETS ExpiringDictionary
        EXPORT ExpiringDictionaryTargets
        LIBRARY DESTINATION lib
        ARCHIVE DESTINATION lib
        RUNTIME DESTINATION bin
        INCLUDES DESTINATION include)
install(DIRECTORY include/ DESTINATION include)
install(EXPORT ExpiringDictionaryTargets
        FILE ExpiringDictionaryTargets.cmake
        NAMESPACE ExpiringDictionary::
        DESTINATION lib/cmake/ExpiringDictionary)

# Generate config file for find_package support
include(CMakePackageConfigHelpers)
write_basic_package_version_file(
    "${CMAKE_CURRENT_BINARY_DIR}/ExpiringDictionaryConfigVersion.cmake"
    VERSION ${PROJECT_VERSION}
    COMPATIBILITY SameMajorVersion
)
configure_file(cmake/ExpiringDictionaryConfig.cmake.in
               "${CMAKE_CURRENT_BINARY_DIR}/ExpiringDictionaryConfig.cmake"
               @ONLY)
install(FILES
    "${CMAKE_CURRENT_BINARY_DIR}/ExpiringDictionaryConfig.cmake"
    "${CMAKE_CURRENT_BINARY_DIR}/ExpiringDictionaryConfigVersion.cmake"
    DESTINATION lib/cmake/ExpiringDictionary)
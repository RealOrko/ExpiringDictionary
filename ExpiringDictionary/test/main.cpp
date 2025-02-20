// test/main.cpp
#include <ExpiringDictionary.h>
#include <iostream>
#include <string>
#include <chrono>
#include <thread>

int main() {
    ExpiringDictionary<std::string, std::string> dict;

    // Insert a key with 2-second expiry
    dict.insert("key1", "value1", std::chrono::seconds(2));
    
    // Immediate lookup
    std::string value;
    if (dict.try_get("key1", value)) {
        std::cout << "Found: " << value << "\n";
    } else {
        std::cout << "Key not found or expired\n";
    }

    // Wait 3 seconds to test expiry
    std::this_thread::sleep_for(std::chrono::seconds(3));
    if (dict.try_get("key1", value)) {
        std::cout << "Found: " << value << "\n";
    } else {
        std::cout << "Key not found or expired\n";
    }

    return 0;
}
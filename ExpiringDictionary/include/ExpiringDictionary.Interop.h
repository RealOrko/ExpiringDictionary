#include <msclr/marshal_cppstd.h>
#include "ExpiringDictionary.h"

using namespace System;
using namespace System::Runtime::InteropServices;

public ref class ExpiringDictionaryWrapper {
private:
    ExpiringDictionary<std::string, std::string>* nativeDict;

public:
    // Constructor
    ExpiringDictionaryWrapper() {
        nativeDict = new ExpiringDictionary<std::string, std::string>();
    }

    // Destructor for deterministic cleanup
    ~ExpiringDictionaryWrapper() {
        delete nativeDict;
        nativeDict = nullptr;
    }

    // Finalizer for garbage collection
    !ExpiringDictionaryWrapper() {
        delete nativeDict;
    }

    // Insert a key-value pair with expiry duration
    void Insert(String^ key, String^ value, TimeSpan duration) {
        std::string nativeKey = msclr::interop::marshal_as<std::string>(key);
        std::string nativeValue = msclr::interop::marshal_as<std::string>(value);
        auto expiry_duration = std::chrono::milliseconds(static_cast<long long>(duration.TotalMilliseconds));
        nativeDict->insert(nativeKey, nativeValue, expiry_duration);
    }

    // Try to get a value; returns false if not found or expired
    bool TryGet(String^ key, [Out] String^% value) {
        std::string nativeKey = msclr::interop::marshal_as<std::string>(key);
        std::string nativeValue;
        bool result = nativeDict->try_get(nativeKey, nativeValue);
        value = result ? msclr::interop::marshal_as<String^>(nativeValue) : nullptr;
        return result;
    }

    // Remove a key
    void Remove(String^ key) {
        std::string nativeKey = msclr::interop::marshal_as<std::string>(key);
        nativeDict->remove(nativeKey);
    }

    // Check if a key exists and hasn't expired
    bool ContainsKey(String^ key) {
        std::string nativeKey = msclr::interop::marshal_as<std::string>(key);
        return nativeDict->contains_key(nativeKey);
    }
};
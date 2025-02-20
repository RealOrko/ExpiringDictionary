#include "ExpiringDictionary.h"
#include <cstring> // Added for strncpy

extern "C" {
    EXPORT_API StringDict* CreateDictionary() {
        return new StringDict();
    }

    EXPORT_API void DestroyDictionary(StringDict* dict) {
        delete dict;
    }

    EXPORT_API void Insert(StringDict* dict, const char* key, const char* value, int64_t expiryMs) {
        if (dict && key && value) {
            dict->insert(std::string(key), std::string(value), std::chrono::milliseconds(expiryMs));
        }
    }

    EXPORT_API int TryGet(StringDict* dict, const char* key, char* value, int valueSize) {
        if (!dict || !key || !value || valueSize <= 0) return 0;
        std::string result;
        bool success = dict->try_get(std::string(key), result);
        if (success) {
            strncpy(value, result.c_str(), valueSize - 1);
            value[valueSize - 1] = '\0'; // Ensure null termination
        }
        return success ? 1 : 0;
    }

    EXPORT_API void Remove(StringDict* dict, const char* key) {
        if (dict && key) {
            dict->remove(std::string(key));
        }
    }

    EXPORT_API int ContainsKey(StringDict* dict, const char* key) {
        if (!dict || !key) return 0;
        return dict->contains_key(std::string(key)) ? 1 : 0;
    }
}
#ifndef EXPIRING_DICTIONARY_H
#define EXPIRING_DICTIONARY_H

#include <unordered_map>
#include <set>
#include <chrono>
#include <utility>
#include <string>

#ifdef _WIN32
#define EXPORT_API __declspec(dllexport)
#else
#define EXPORT_API __attribute__((visibility("default")))
#endif

// Template class remains the same
template <typename Key, typename Value>
class ExpiringDictionary {
private:
    using time_point = std::chrono::steady_clock::time_point;
    using duration = std::chrono::steady_clock::duration;
    using expiry_set_t = std::set<std::pair<time_point, Key>>;
    using main_map_t = std::unordered_map<Key, std::pair<Value, typename expiry_set_t::iterator>>;

    main_map_t main_map;
    expiry_set_t expiry_set;
    time_point last_cleanup_time;
    duration cleanup_interval;

    void cleanup() {
        auto current_time = std::chrono::steady_clock::now();
        while (!expiry_set.empty() && expiry_set.begin()->first <= current_time) {
            auto key = expiry_set.begin()->second;
            main_map.erase(key);
            expiry_set.erase(expiry_set.begin());
        }
        last_cleanup_time = current_time;
    }

public:
    ExpiringDictionary() 
        : last_cleanup_time(std::chrono::steady_clock::now()),
          cleanup_interval(std::chrono::seconds(1)) {}

    void insert(const Key& key, const Value& value, duration expiry_duration) {
        auto current_time = std::chrono::steady_clock::now();
        if (current_time > last_cleanup_time + cleanup_interval) {
            cleanup();
        }
        auto expiry_time = current_time + expiry_duration;
        auto it = expiry_set.insert({expiry_time, key}).first;
        main_map[key] = {value, it};
    }

    bool try_get(const Key& key, Value& value) {
        auto current_time = std::chrono::steady_clock::now();
        if (current_time > last_cleanup_time + cleanup_interval) {
            cleanup();
        }
        auto it = main_map.find(key);
        if (it == main_map.end()) return false;
        if (current_time > it->second.second->first) {
            expiry_set.erase(it->second.second);
            main_map.erase(it);
            return false;
        }
        value = it->second.first;
        return true;
    }

    void remove(const Key& key) {
        auto current_time = std::chrono::steady_clock::now();
        if (current_time > last_cleanup_time + cleanup_interval) {
            cleanup();
        }
        auto it = main_map.find(key);
        if (it != main_map.end()) {
            expiry_set.erase(it->second.second);
            main_map.erase(it);
        }
    }

    bool contains_key(const Key& key) {
        Value dummy;
        return try_get(key, dummy);
    }
};

// C-style API for P/Invoke
using StringDict = ExpiringDictionary<std::string, std::string>;

extern "C" {
    EXPORT_API StringDict* CreateDictionary();
    EXPORT_API void DestroyDictionary(StringDict* dict);
    EXPORT_API void Insert(StringDict* dict, const char* key, const char* value, int64_t expiryMs);
    EXPORT_API int TryGet(StringDict* dict, const char* key, char* value, int valueSize);
    EXPORT_API void Remove(StringDict* dict, const char* key);
    EXPORT_API int ContainsKey(StringDict* dict, const char* key);
}

#endif // EXPIRING_DICTIONARY_H
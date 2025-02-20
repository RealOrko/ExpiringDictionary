using System.Runtime.InteropServices;
using System.Text;

namespace ExpiringDictionaryWrapper;

public class ExpiringDictionaryWrapper<TKey, TValue> : IDisposable where TValue : class
{
    private IntPtr _dict;
    private bool _disposed = false;
    private const int MaxValueSize = 1024;

    private readonly Func<TKey, string> _keyToString;
    private readonly Func<string, TKey> _stringToKey;
    private readonly Func<TValue, string> _valueToString;
    private readonly Func<string, TValue> _stringToValue;

    public ExpiringDictionaryWrapper(
        Func<TKey, string> keyToString = null,
        Func<string, TKey> stringToKey = null,
        Func<TValue, string> valueToString = null,
        Func<string, TValue> stringToValue = null)
    {
        _dict = NativeMethods.CreateDictionary();

        _keyToString = keyToString ?? (k => k?.ToString() ?? throw new ArgumentNullException(nameof(k)));
        _stringToKey = stringToKey ?? DefaultStringToKey;
        _valueToString = valueToString ?? (v => v?.ToString() ?? throw new ArgumentNullException(nameof(v)));
        _stringToValue = stringToValue ?? DefaultStringToValue;
    }

    private TKey DefaultStringToKey(string s)
    {
        if (typeof(TKey) == typeof(string)) return (TKey)(object)s;
        if (typeof(TKey) == typeof(int) && int.TryParse(s, out int i)) return (TKey)(object)i;
        if (typeof(TKey) == typeof(double) && double.TryParse(s, out double d)) return (TKey)(object)d;
        throw new ArgumentException($"Cannot convert '{s}' to {typeof(TKey).Name}");
    }

    private TValue DefaultStringToValue(string s)
    {
        // Since TValue is constrained to class, we only handle reference types
        if (typeof(TValue) == typeof(string)) return (TValue)(object)s;
        // Add custom deserialization logic for other class types if needed
        throw new ArgumentException($"Cannot convert '{s}' to {typeof(TValue).Name}. Provide a custom stringToValue function.");
    }

    private void CheckDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ExpiringDictionaryWrapper<TKey, TValue>));
    }

    public void Insert(TKey key, TValue value, TimeSpan expiry)
    {
        CheckDisposed();
        string keyStr = _keyToString(key);
        string valueStr = _valueToString(value);
        NativeMethods.Insert(_dict, keyStr, valueStr, (long)expiry.TotalMilliseconds);
    }

    public bool TryGet(TKey key, out TValue value)
    {
        CheckDisposed();
        string keyStr = _keyToString(key);
        byte[] buffer = new byte[MaxValueSize];
        int result = NativeMethods.TryGet(_dict, keyStr, buffer, buffer.Length);
        if (result != 0)
        {
            string valueStr = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
            value = _stringToValue(valueStr);
            return true;
        }
        value = null; // Since TValue is a class, default is null
        return false;
    }

    public void Remove(TKey key)
    {
        CheckDisposed();
        string keyStr = _keyToString(key);
        NativeMethods.Remove(_dict, keyStr);
    }

    public bool ContainsKey(TKey key)
    {
        CheckDisposed();
        string keyStr = _keyToString(key);
        return NativeMethods.ContainsKey(_dict, keyStr) != 0;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_dict != IntPtr.Zero)
            {
                NativeMethods.DestroyDictionary(_dict);
                _dict = IntPtr.Zero;
            }
            _disposed = true;
        }
    }
}
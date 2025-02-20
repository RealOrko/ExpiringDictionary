using System.Runtime.InteropServices;
using System.Text;

namespace ExpiringDictionaryWrapper;

public class ExpiringDictionaryWrapper : IDisposable
{
    private const string DllName = "ExpiringDictionary";

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr CreateDictionary();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void DestroyDictionary(IntPtr dict);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void Insert(IntPtr dict, string key, string value, long expiryMs);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int TryGet(IntPtr dict, string key, [Out] byte[] value, int valueSize);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void Remove(IntPtr dict, string key);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    private static extern int ContainsKey(IntPtr dict, string key);

    private IntPtr _dict;
    private bool _disposed = false;
    private const int MaxValueSize = 1024;

    public ExpiringDictionaryWrapper()
    {
        _dict = CreateDictionary();
    }

    private void CheckDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ExpiringDictionaryWrapper));
    }

    public void Insert(string key, string value, TimeSpan expiry)
    {
        CheckDisposed();
        Insert(_dict, key, value, (long)expiry.TotalMilliseconds);
    }

    public bool TryGet(string key, out string value)
    {
        CheckDisposed(); // Ensure this is called before any native call
        byte[] buffer = new byte[MaxValueSize];
        int result = TryGet(_dict, key, buffer, buffer.Length);
        value = result != 0 ? Encoding.UTF8.GetString(buffer).TrimEnd('\0') : null;
        return result != 0;
    }

    public void Remove(string key)
    {
        CheckDisposed();
        Remove(_dict, key);
    }

    public bool ContainsKey(string key)
    {
        CheckDisposed();
        return ContainsKey(_dict, key) != 0;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_dict != IntPtr.Zero)
            {
                DestroyDictionary(_dict);
                _dict = IntPtr.Zero;
            }
            _disposed = true;
        }
    }
}
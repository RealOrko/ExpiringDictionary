using System.Runtime.InteropServices;

internal static class NativeMethods
{
    private const string DllName = "ExpiringDictionary";

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateDictionary();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void DestroyDictionary(IntPtr dict);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Insert(IntPtr dict, string key, string value, long expiryMs);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int TryGet(IntPtr dict, string key, [Out] byte[] value, int valueSize);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Remove(IntPtr dict, string key);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int ContainsKey(IntPtr dict, string key);
}
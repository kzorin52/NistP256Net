using System.Reflection;
using System.Runtime.InteropServices;

namespace NistP256Net;

public static partial class NistP256
{ 
    private const string LibraryName = "NistP256";

    static NistP256()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), LoadLibrary);
    }
    private static nint LoadLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        string platform;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            libraryName = $"lib{libraryName}.so";
            platform = "linux";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            libraryName = $"{libraryName}.dll";
            platform = "win";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            libraryName = $"lib{libraryName}.dylib";
            platform = "osx";
        }
        else
            throw new PlatformNotSupportedException();

        if (NativeLibrary.TryLoad(libraryName, assembly, searchPath, out var handle))
            return handle;

        var arch = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();

        return NativeLibrary.Load(Path.Combine("runtimes", $"{platform}-{arch}", "native", libraryName), assembly, searchPath);
    }

#pragma warning disable CA1401

    [LibraryImport(LibraryName)]
    public static unsafe partial void private_to_public_key(byte* secretKey, byte* publicKey);

#pragma warning restore CA1401

    /// <summary>
    /// Get compressed NistP256 public key
    /// </summary>
    /// <param name="privateKey">private key</param>
    /// <returns>compressed public key</returns>
    public static unsafe byte[] GetPublicKey(ReadOnlySpan<byte> privateKey)
    {
        Span<byte> publicKey = stackalloc byte[33];

        fixed (byte* pubKeyPtr = &MemoryMarshal.GetReference(publicKey), prvKeyPtr = &MemoryMarshal.GetReference(privateKey))
        {
            private_to_public_key(prvKeyPtr, pubKeyPtr);
        }

        return publicKey.ToArray();
    }
}
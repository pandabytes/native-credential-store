using System.Runtime.InteropServices;
using NativeCredentialStore.Platform;

namespace NativeCredentialStore;

public static class CredentialStoreFactory
{
  public static INativeCredentialStore GetCredentialStore(
    OsArchitecture? osArchitecture = null,
    CredentialService? credentialService = null
  )
  {
    var osPlatform = GetOsPlatform();
    osArchitecture ??= GetOsArchitecture();
    credentialService ??= GetCredentialService(osPlatform);
    var credentialHelper = new CredentialHelperExecutable(credentialService, osPlatform, osArchitecture);
    return new NativeCredentialStore(credentialHelper);
  }

  private static OsPlatform GetOsPlatform()
  {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      return OsPlatform.Windows;
    }

    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
      return OsPlatform.Mac;
    }

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
      return OsPlatform.Linux;
    }

    throw new NotSupportedException("Unknown OS.");
  }

  private static OsArchitecture GetOsArchitecture()
  {
    var actualArchitecture = RuntimeInformation.OSArchitecture;
    return actualArchitecture switch
    {
      Architecture.X64 => OsArchitecture.Amd64,
      Architecture.Arm64 => OsArchitecture.Arm64,
      Architecture.Armv6 => OsArchitecture.ArmV6,
      Architecture.Ppc64le => OsArchitecture.Ppc64le,
      Architecture.S390x => OsArchitecture.S390x,
      _ => throw new NotSupportedException($"OS architecture \"{actualArchitecture}\" not supported.")
    };
  }

  private static CredentialService GetCredentialService(OsPlatform osPlatform)
  {
    if (osPlatform == OsPlatform.Windows)
    {
      return CredentialService.WindowsCredential;
    }

    if (osPlatform == OsPlatform.Mac)
    {
      return CredentialService.Keychain;
    }

    if (osPlatform == OsPlatform.Linux)
    {
      // Default to Pass
      return CredentialService.Pass;
    }

    // Should never reach here
    throw new NotSupportedException($"OS \"{osPlatform}\" not supported.");
  }
}

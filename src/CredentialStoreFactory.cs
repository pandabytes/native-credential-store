using System.Runtime.InteropServices;

namespace NativeCredentialStore;

/// <summary>
/// Factory class to get <see cref="INativeCredentialStore"/>.
/// </summary>
public static class CredentialStoreFactory
{
  /// <summary>
  /// Attempt to get a native credential service
  /// based on the device that this library
  /// runs on. Use the parameters to for finer control.
  /// </summary>
  /// <param name="osArchitecture">Manually specify the OS architecture.</param>
  /// <param name="credentialService">Manually specify the credential service.</param>
  /// <exception cref="ArgumentException">
  /// Thrown when this methods fails to get an <see cref="INativeCredentialStore"/> object.
  /// </exception>
  /// <returns><see cref="INativeCredentialStore"/> object.</returns>
  public static INativeCredentialStore GetCredentialStore(
    OsArchitecture? osArchitecture = null,
    CredentialService? credentialService = null
  )
  {
    var osPlatform = GetOsPlatform();
    osArchitecture ??= GetOsArchitecture();
    credentialService ??= GetCredentialService(osPlatform);
    var credentialHelper = new DockerCredentialHelperExecutable(credentialService, osPlatform, osArchitecture);
    return new DockerCredentialHelperImplementation(credentialHelper);
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

using static NativeCredentialStore.CredentialStoreFactory;

namespace Tests.NativeCredentialStore;

public class NativeCredentialStoreFactoryTests
{
  [PlatformTrait(Platform.All)]
  [Fact]
  public void GetCredentialStore_DefaultArguments_ReturnsCredentialStore()
  {
    // Act
    var credentialStore = GetDockerCredentialHelper();

    // Assert
    Assert.NotNull(credentialStore);
    Assert.True(File.Exists(credentialStore.ExecutableFilePath));
    Assert.Contains(credentialStore.Version, credentialStore.ExecutableFilePath);
  }

  [PlatformTrait(Platform.MacOS)]
  [Fact]
  public void GetCredentialStore_WrongOsArchictectureOnMac_ThrowsException()
  {
    Assert.Throws<ArgumentException>(()
      => GetDockerCredentialHelper(osArchitecture: OsArchitecture.Ppc64le));
  }

  [PlatformTrait(Platform.MacOS)]
  [Fact]
  public void GetCredentialStore_WrongCredentialServiceOnMac_ThrowsException()
  {
    Assert.Throws<ArgumentException>(() 
      => GetDockerCredentialHelper(credentialService: CredentialService.WindowsCredential));
  }

  [PlatformTrait(Platform.Windows)]
  [Fact]
  public void GetCredentialStore_WrongOsArchictectureOnWindows_ThrowsException()
  {
    Assert.Throws<ArgumentException>(()
      => GetDockerCredentialHelper(osArchitecture: OsArchitecture.Ppc64le));
  }

  [PlatformTrait(Platform.Windows)]
  [Fact]
  public void GetCredentialStore_WrongCredentialServiceOnWindows_ThrowsException()
  {
    Assert.Throws<ArgumentException>(() 
      => GetDockerCredentialHelper(credentialService: CredentialService.Keychain));
  }

  [PlatformTrait(Platform.Linux)]
  [Fact]
  public void GetCredentialStore_WrongCredentialServiceOnLinux_ThrowsException()
  {
    Assert.Throws<ArgumentException>(()
      => GetDockerCredentialHelper(credentialService: CredentialService.Keychain));

    Assert.Throws<ArgumentException>(() 
      => GetDockerCredentialHelper(credentialService: CredentialService.WindowsCredential));
  }
}

using static NativeCredentialStore.CredentialStoreFactory;

namespace Tests.NativeCredentialStore;

public class NativeCredentialStoreFactoryTests
{
  [PlatformTrait(Platform.All)]
  [Fact]
  public void GetDockerCredentialHelper_DefaultArguments_ReturnsCredentialStore()
  {
    // Arrange
    const string currentDockerCredHelperVersion = "v0.8.2";

    // Act
    var credentialStore = GetDockerCredentialHelper();

    // Assert
    Assert.NotNull(credentialStore);
    Assert.True(File.Exists(credentialStore.ExecutableFilePath));
    Assert.Contains(credentialStore.Version, credentialStore.ExecutableFilePath);
    Assert.Equal(currentDockerCredHelperVersion, credentialStore.Version);
  }

  [PlatformTrait(Platform.MacOS)]
  [Fact]
  public void GetDockerCredentialHelper_WrongOsArchictectureOnMac_ThrowsException()
  {
    Assert.Throws<ArgumentException>(()
      => GetDockerCredentialHelper(osArchitecture: OsArchitecture.Ppc64le));
  }

  [PlatformTrait(Platform.MacOS)]
  [Fact]
  public void GetDockerCredentialHelper_WrongCredentialServiceOnMac_ThrowsException()
  {
    Assert.Throws<ArgumentException>(() 
      => GetDockerCredentialHelper(credentialService: CredentialService.WindowsCredential));
  }

  [PlatformTrait(Platform.Windows)]
  [Fact]
  public void GetDockerCredentialHelper_WrongOsArchictectureOnWindows_ThrowsException()
  {
    Assert.Throws<ArgumentException>(()
      => GetDockerCredentialHelper(osArchitecture: OsArchitecture.Ppc64le));
  }

  [PlatformTrait(Platform.Windows)]
  [Fact]
  public void GetDockerCredentialHelper_WrongCredentialServiceOnWindows_ThrowsException()
  {
    Assert.Throws<ArgumentException>(() 
      => GetDockerCredentialHelper(credentialService: CredentialService.Keychain));
  }

  [PlatformTrait(Platform.Linux)]
  [Fact]
  public void GetDockerCredentialHelper_WrongCredentialServiceOnLinux_ThrowsException()
  {
    Assert.Throws<ArgumentException>(()
      => GetDockerCredentialHelper(credentialService: CredentialService.Keychain));

    Assert.Throws<ArgumentException>(() 
      => GetDockerCredentialHelper(credentialService: CredentialService.WindowsCredential));
  }
}

using NativeCredentialStore.Platform;
using static NativeCredentialStore.CredentialStoreFactory;

namespace IntegrationTests.NativeCredentialStore;

public class NativeCredentialStoreFactoryTests
{
  [Fact]
  public void GetCredentialStore_DefaultArguments_ReturnsCredentialStore()
  {
    // Act
    var credentialStore = GetCredentialStore();

    // Assert
    Assert.NotNull(credentialStore);
    Assert.True(File.Exists(credentialStore.ExecutableFilePath));
  }

  [Trait("Platform", "Mac")]
  [Fact]
  public void GetCredentialStore_WrongOsArchictecture_ThrowsException()
  {
    Assert.Throws<ArgumentException>(() 
      => GetCredentialStore(osArchitecture: OsArchitecture.Ppc64le));
  }

  [Trait("Platform", "Mac")]
  [Fact]
  public void GetCredentialStore_WrongCredentialService_ThrowsException()
  {
    Assert.Throws<ArgumentException>(() 
      => GetCredentialStore(credentialService: CredentialService.WindowsCredential));
  }
}

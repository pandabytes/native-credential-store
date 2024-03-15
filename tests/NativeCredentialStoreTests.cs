using Xunit.Abstractions;

namespace IntegrationTests.NativeCredentialStore;

public class NativeCredentialStoreTests : IAsyncLifetime
{
  private readonly INativeCredentialStore _credentialStore;

  private static readonly Credentials Credential = new()
  {
    ServerURL = "http://localhost",
    Username = "foo@email.com",
    Secret = "password"
  };

  private readonly ITestOutputHelper _output;

  public NativeCredentialStoreTests(ITestOutputHelper output)
  {
    _output = output;

    var serviceProvider = new ServiceCollection()
      .AddCredentialStore()
      .BuildServiceProvider();

    _credentialStore = serviceProvider.GetRequiredService<INativeCredentialStore>();
  }

  public Task InitializeAsync() => Task.CompletedTask;

  public async Task DisposeAsync()
  {
    var credentials = await _credentialStore.ListAsync();
    foreach (var serverUrl in credentials.Keys)
    {
      await _credentialStore.EraseAsync(serverUrl);
    }
  }

  /// <summary>
  /// This also tests <see cref="INativeCredentialStore.GetAsync(string, CancellationToken)"/>.
  /// </summary>
  [Fact]
  public async Task StoreAsync_StoreCredential_CredentialIsStored()
  {
    // Act
    await _credentialStore.StoreAsync(Credential);

    // Assert
    var storedCredentials = await _credentialStore.GetAsync(Credential.ServerURL);
    Assert.Equal(Credential, storedCredentials);
  }

  [Fact]
  public async Task StoreAsync_UpdateCredential_CredentialIsUpdated()
  {
    // Arrange
    await _credentialStore.StoreAsync(Credential);

    // Act
    var updateCredential = Credential with { Username = "bar@email.com" };
    await _credentialStore.StoreAsync(updateCredential);

    // Assert
    var storedCredentials = await _credentialStore.GetAsync(Credential.ServerURL);
    Assert.Equal(updateCredential, storedCredentials);
  }

  [Fact]
  public async Task GetAsync_EmptyServerUrl_ThrowsException()
    => await Assert.ThrowsAsync<ArgumentException>(() => _credentialStore.GetAsync(string.Empty));

  [Fact]
  public async Task GetAsync_ServerUrlNotExist_ThrowsException()
    => await Assert.ThrowsAsync<CommandException>(() => _credentialStore.GetAsync("foo"));

  [Fact]
  public async Task EraseAsync_EmptyServerUrl_ThrowsException()
    => await Assert.ThrowsAsync<ArgumentException>(() => _credentialStore.EraseAsync(string.Empty));

  [Fact]
  public async Task EraseAsync_ServerUrlNotExist_ThrowsException()
    => await Assert.ThrowsAsync<CommandException>(() => _credentialStore.EraseAsync("foo"));

  [Fact]
  public async Task EraseAsync_ServerUrlExists_CredentialIsRemoved()
  {
    // Arrange
    await _credentialStore.StoreAsync(Credential);

    // Act
    await _credentialStore.EraseAsync(Credential.ServerURL);

    // Assert
    var credentials = await _credentialStore.ListAsync();
    Assert.Empty(credentials);
  }

  [InlineData(0)]
  [InlineData(1)]
  [InlineData(5)]
  [InlineData(10)]
  [Theory]
  public async Task ListAsync_CheckStoredCredentials_CountMatch(int count)
  {
    // Arrange
    var credentialObjs = new List<Credentials>();
    for (int i = 0; i < count; i++)
    {
      var serverUrl = Credential.ServerURL + $"/v{i}";
      var username = Credential.Username + i.ToString();
      var newCredential = Credential with { ServerURL = serverUrl, Username = username };

      await _credentialStore.StoreAsync(newCredential);
      credentialObjs.Add(newCredential);
    }

    // Act
    var credentials = await _credentialStore.ListAsync();

    // Assert
    Assert.Equal(count, credentials.Count);

    foreach (var credential in credentialObjs)
    {
      Assert.Contains(credential.ServerURL, credentials);
      Assert.Equal(credential.Username, credentials[credential.ServerURL]);
    }
  }
}

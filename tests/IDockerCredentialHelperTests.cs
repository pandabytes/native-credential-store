using Xunit.Abstractions;

namespace Tests.NativeCredentialStore;

[PlatformTrait(Platform.All)]
public class IDockerCredentialHelperTests : IAsyncLifetime
{
  private readonly IDockerCredentialHelper _dockerCredentialStore;

  private static readonly Credentials Credential = new()
  {
    ServerURL = "http://nativecredentialstore.com",
    Username = "foo@email.com",
    Secret = "password"
  };

  private readonly ITestOutputHelper _output;

  public IDockerCredentialHelperTests(ITestOutputHelper output)
  {
    _output = output;

    var serviceProvider = new ServiceCollection()
      .AddDockerCredentialHelper()
      .BuildServiceProvider();

    _dockerCredentialStore = serviceProvider.GetRequiredService<IDockerCredentialHelper>();
  }

  public Task InitializeAsync() => Task.CompletedTask;

  public async Task DisposeAsync()
  {
    var credentials = await _dockerCredentialStore.ListAsync();
    foreach (var serverUrl in credentials.Keys)
    {
      await _dockerCredentialStore.EraseAsync(serverUrl);
    }
  }

  /// <summary>
  /// This also tests <see cref="INativeCredentialStore.GetAsync(string, CancellationToken)"/>.
  /// </summary>
  [Fact]
  public async Task StoreAsync_StoreCredential_CredentialIsStored()
  {
    // Act
    await _dockerCredentialStore.StoreAsync(Credential);

    // Assert
    var storedCredentials = await _dockerCredentialStore.GetAsync(Credential.ServerURL);
    Assert.Equal(Credential, storedCredentials);
  }

  [Fact]
  public async Task StoreAsync_UpdateCredential_CredentialIsUpdated()
  {
    // Arrange
    await _dockerCredentialStore.StoreAsync(Credential);

    // Act
    var updateCredential = Credential with { Username = "bar@email.com" };
    await _dockerCredentialStore.StoreAsync(updateCredential);

    // Assert
    var storedCredentials = await _dockerCredentialStore.GetAsync(Credential.ServerURL);
    Assert.Equal(updateCredential, storedCredentials);
  }

  [Fact]
  public async Task GetAsync_EmptyServerUrl_ThrowsException()
    => await Assert.ThrowsAsync<ArgumentException>(() => _dockerCredentialStore.GetAsync(string.Empty));

  [Fact]
  public async Task GetAsync_ServerUrlNotExist_ThrowsException()
    => await Assert.ThrowsAsync<CommandException>(() => _dockerCredentialStore.GetAsync("foo"));

  [Fact]
  public async Task EraseAsync_EmptyServerUrl_ThrowsException()
    => await Assert.ThrowsAsync<ArgumentException>(() => _dockerCredentialStore.EraseAsync(string.Empty));

  [Fact]
  public async Task EraseAsync_ServerUrlNotExist_NothingHappens()
    => await _dockerCredentialStore.EraseAsync("foo");

  [Fact]
  public async Task EraseAsync_ServerUrlExists_CredentialIsRemoved()
  {
    // Arrange
    await _dockerCredentialStore.StoreAsync(Credential);

    // Act
    await _dockerCredentialStore.EraseAsync(Credential.ServerURL);

    // Assert
    var credentials = await _dockerCredentialStore.ListAsync();
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

      await _dockerCredentialStore.StoreAsync(newCredential);
      credentialObjs.Add(newCredential);
    }

    // Act
    // This may return additional credentials that were
    // setup by build environment (like github actions)
    // so we need to filter those out
    IDictionary<string, string> credentials = (await _dockerCredentialStore.ListAsync())
      .Where(pair => pair.Key.StartsWith(Credential.ServerURL))
      .ToDictionary(pair => pair.Key, pair => pair.Value);

    // Assert
    Assert.Equal(count, credentials.Count);

    foreach (var credential in credentialObjs)
    {
      Assert.Contains(credential.ServerURL, credentials);
      Assert.Equal(credential.Username, credentials[credential.ServerURL]);
    }
  }
}

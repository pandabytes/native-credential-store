using Xunit.Abstractions;

namespace Tests.NativeCredentialStore;

[PlatformTrait(Platform.All)]
public class IDockerCredentialHelperTests : IAsyncLifetime
{
  private readonly IDockerCredentialHelper _dockerCredentialStore;

  private static readonly Credentials TestCredential = new()
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
    var credentials = await ListCredentialsAsync();
    foreach (var serverUrl in credentials.Keys)
    {
      await _dockerCredentialStore.EraseAsync(serverUrl);
    }
  }

  /// <summary>
  /// This also tests <see cref="IDockerCredentialHelper.GetAsync(string, CancellationToken)"/>.
  /// </summary>
  [Fact]
  public async Task StoreAsync_StoreCredential_CredentialIsStored()
  {
    // Act
    await _dockerCredentialStore.StoreAsync(TestCredential);

    // Assert
    var storedCredentials = await _dockerCredentialStore.GetAsync(TestCredential.ServerURL);
    Assert.Equal(TestCredential, storedCredentials);
  }

  [Fact]
  public async Task StoreAsync_UpdateCredential_CredentialIsUpdated()
  {
    // Arrange
    await _dockerCredentialStore.StoreAsync(TestCredential);

    // Act
    var updateCredential = TestCredential with { Username = "bar@email.com" };
    await _dockerCredentialStore.StoreAsync(updateCredential);

    // Assert
    var storedCredentials = await _dockerCredentialStore.GetAsync(TestCredential.ServerURL);
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
    await _dockerCredentialStore.StoreAsync(TestCredential);

    // Act
    await _dockerCredentialStore.EraseAsync(TestCredential.ServerURL);

    // Assert
    var credentials = await ListCredentialsAsync();
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
      var serverUrl = TestCredential.ServerURL + $"/v{i}";
      var username = TestCredential.Username + i.ToString();
      var newCredential = TestCredential with { ServerURL = serverUrl, Username = username };

      await _dockerCredentialStore.StoreAsync(newCredential);
      credentialObjs.Add(newCredential);
    }

    // Act
    var credentials = await ListCredentialsAsync();

    // Assert
    Assert.Equal(count, credentials.Count);

    foreach (var credential in credentialObjs)
    {
      Assert.Contains(credential.ServerURL, credentials);
      Assert.Equal(credential.Username, credentials[credential.ServerURL]);
    }
  }

  private async Task<IDictionary<string, string>> ListCredentialsAsync()
  {
    // This may return additional credentials that were
    // setup by build environment (like github actions)
    // so we need to filter those out
    return (await _dockerCredentialStore.ListAsync())
      .Where(pair => pair.Key.StartsWith(TestCredential.ServerURL))
      .ToDictionary(pair => pair.Key, pair => pair.Value);
  }
}

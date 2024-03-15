namespace NativeCredentialStore;

/// <summary>
/// Interface to interact with native credential store.
/// This mirrors the interface 
/// https://pkg.go.dev/github.com/docker/docker-credential-helpers/client
/// </summary>
public interface INativeCredentialStore
{
  /// <summary>
  /// Path to where the docker-credential-helper executable lives.
  /// </summary>
  string ExecutableFilePath { get; }

  /// <summary>
  /// Store <paramref name="credentials"/> in the native credential store.
  /// </summary>
  /// <param name="credentials">Crednetials to be stored.</param>
  /// <param name="cancellationToken">Cancel token.</param>
  /// <exception cref="CommandException"/>
  Task StoreAsync(Credentials credentials, CancellationToken cancellationToken = default);

  /// <summary>
  /// Get the credentials given <paramref name="serverURL"/>.
  /// </summary>
  /// <param name="serverURL">Server url.</param>
  /// <param name="cancellationToken">Cancel token.</param>
  /// <exception cref="CommandException"/>
  /// <returns>Credentials object.</returns>
  Task<Credentials> GetAsync(string serverURL, CancellationToken cancellationToken = default);

  /// <summary>
  /// Erase the credentials given <paramref name="serverURL"/>.
  /// </summary>
  /// <param name="serverURL">Server url.</param>
  /// <param name="cancellationToken">Cancel token.</param>
  /// <exception cref="CommandException"/>
  Task EraseAsync(string serverURL, CancellationToken cancellationToken = default);

  /// <summary>
  /// List the credentials stored in the native credential store.
  /// </summary>
  /// <param name="cancellationToken">Cancel token.</param>
  /// <exception cref="CommandException"/>
  /// <returns>
  /// A dictionary where the key is the server url
  /// and the value is the username.
  /// </returns>
  Task<IDictionary<string, string>> ListAsync(CancellationToken cancellationToken = default);
}

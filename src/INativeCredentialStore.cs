namespace NativeCredentialStore;

/// <summary>
/// Interface to interact with native credential storage system.
/// </summary>
public interface INativeCredentialStore<TCredential> where TCredential : Credentials
{
  /// <summary>
  /// Store <paramref name="credentials"/> in the native credential store.
  /// This method can also be used to update an existing <paramref name="credentials"/>.
  /// </summary>
  /// <param name="credentials">Crednetials to be stored.</param>
  /// <param name="cancellationToken">Cancel token.</param>
  /// <exception cref="CommandException"/>
  Task StoreAsync(TCredential credentials, CancellationToken cancellationToken = default);

  /// <summary>
  /// Get the credentials given <paramref name="serverURL"/>.
  /// This method throws an exception when <paramref name="serverURL"/>
  /// doesn't exist.
  /// </summary>
  /// <param name="serverURL">Server url.</param>
  /// <param name="cancellationToken">Cancel token.</param>
  /// <exception cref="ArgumentException"/>
  /// <exception cref="CommandException"/>
  /// <returns>Credentials object.</returns>
  Task<TCredential> GetAsync(string serverURL, CancellationToken cancellationToken = default);

  /// <summary>
  /// Erase the credentials given <paramref name="serverURL"/>.
  /// This method is idempotent, meaning if the <paramref name="serverURL"/>
  /// doesn't exist, it will not throw an exception.
  /// </summary>
  /// <param name="serverURL">Server url.</param>
  /// <param name="cancellationToken">Cancel token.</param>
  /// <exception cref="ArgumentException"/>
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

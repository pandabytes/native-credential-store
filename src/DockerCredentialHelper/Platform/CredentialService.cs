namespace NativeCredentialStore.DockerCredentialHelper.Platform;

/// <summary>
/// Provide an enum list of available credential service.
/// </summary>
public sealed class CredentialService : StringEnum
{
  private CredentialService(string value) : base(value) {}

  /// <summary>
  /// Keychain on Mac OS.
  /// </summary>
  public static readonly CredentialService Keychain = new("osxkeychain");

  /// <summary>
  /// Pass on Linux.
  /// </summary>
  public static readonly CredentialService Pass = new("pass");

  /// <summary>
  /// Secret service on Linux.
  /// </summary>
  public static readonly CredentialService SecretService = new("secretservice");

  /// <summary>
  /// Windows credential manager.
  /// </summary>
  public static readonly CredentialService WindowsCredential = new("wincred");
}

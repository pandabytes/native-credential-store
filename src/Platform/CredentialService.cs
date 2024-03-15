namespace NativeCredentialStore.Platform;

public sealed class CredentialService : StringEnum
{
  private CredentialService(string value) : base(value) {}

  public static readonly CredentialService Keychain = new("osxkeychain");

  public static readonly CredentialService Pass = new("pass");

  public static readonly CredentialService SecretService = new("secretservice");

  public static readonly CredentialService WindowsCredential = new("wincred");
}

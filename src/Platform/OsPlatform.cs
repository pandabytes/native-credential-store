namespace NativeCredentialStore.Platform;

public sealed class OsPlatform : StringEnum
{
  private OsPlatform(string value) : base(value) {}

  public static readonly OsPlatform Mac = new("darwin");

  public static readonly OsPlatform Linux = new("linux");

  public static readonly OsPlatform Windows = new("windows");
}

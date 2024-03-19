namespace NativeCredentialStore.Platform;

/// <summary>
/// Provide an enum list of available OS platform.
/// </summary>
public sealed class OsPlatform : StringEnum
{
  private OsPlatform(string value) : base(value) {}

  /// <summary>
  /// Mac OS.
  /// </summary>
  public static readonly OsPlatform Mac = new("darwin");

  /// <summary>
  /// Linux OS.
  /// </summary>
  public static readonly OsPlatform Linux = new("linux");

  /// <summary>
  /// Windows OS.
  /// </summary>
  public static readonly OsPlatform Windows = new("windows");
}

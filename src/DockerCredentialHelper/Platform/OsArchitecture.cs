namespace NativeCredentialStore.DockerCredentialHelper.Platform;

/// <summary>
/// Provide an enum list of available OS architecture.
/// </summary>
public sealed class OsArchitecture : StringEnum
{
  private OsArchitecture(string value) : base(value) {}

  /// <summary>
  /// AMD 64 achitecture.
  /// </summary>
  public static readonly OsArchitecture Amd64 = new("amd64");

  /// <summary>
  /// ARM 64 achitecture.
  /// </summary>
  public static readonly OsArchitecture Arm64 = new("arm64");

  /// <summary>
  /// AMD V6 achitecture.
  /// </summary>
  public static readonly OsArchitecture ArmV6 = new("armv6");

  /// <summary>
  /// ARM V7 achitecture.
  /// </summary>
  public static readonly OsArchitecture ArmV7 = new("armv7");

  /// <summary>
  /// PPC64LE achitecture.
  /// </summary>
  public static readonly OsArchitecture Ppc64le = new("ppc64le");

  /// <summary>
  /// S390x achitecture.
  /// </summary>
  public static readonly OsArchitecture S390x = new("s390x");
}

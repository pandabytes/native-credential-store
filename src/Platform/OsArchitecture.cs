namespace NativeCredentialStore.Platform;

public sealed class OsArchitecture : StringEnum
{
  private OsArchitecture(string value) : base(value) {}

  public static readonly OsArchitecture Amd64 = new("amd64");

  public static readonly OsArchitecture Arm64 = new("arm64");

  public static readonly OsArchitecture ArmV6 = new("armv6");

  public static readonly OsArchitecture ArmV7 = new("armv7");

  public static readonly OsArchitecture Ppc64le = new("ppc64le");

  public static readonly OsArchitecture S390x = new("s390x");
}

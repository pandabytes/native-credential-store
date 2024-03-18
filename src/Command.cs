namespace NativeCredentialStore;

internal record CommandResult(int ExitCode, string Output, string Error, string ExePath);

internal sealed class Command : StringEnum
{
  public static readonly Command Store = new("store");

  public static readonly Command Get = new("get");

  public static readonly Command Erase = new("erase");

  public static readonly Command List = new("list");

  private Command(string value) : base(value) {}
}

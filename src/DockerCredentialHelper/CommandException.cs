namespace NativeCredentialStore.DockerCredentialHelper;

/// <summary>
/// Exception raised when a command fails.
/// </summary>
public class CommandException : Exception
{
  /// <inheritdoc/>
  public CommandException(string message) : base(message) { }

  /// <inheritdoc/>
  public CommandException(string message, Exception inner) : base(message, inner) { }

  internal CommandException(Command command, CommandResult commandResult)
    : base(CommandExceptionMessage(command, commandResult))
  {}

  internal CommandException(Command command, CommandResult commandResult, Exception innerEx)
    : base(CommandExceptionMessage(command, commandResult), innerEx)
  {}

  private static string CommandExceptionMessage(Command command, CommandResult commandResult)
    => $"Command \"{command}\" failed with exit code {commandResult.ExitCode}. " +
       $"Output: \"{commandResult.Output}\". Error: \"{commandResult.Error}\". " +
       $"ExePath: \"{commandResult.ExePath}\"";
}
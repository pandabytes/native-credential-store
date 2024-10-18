using System.Text.Json;

namespace NativeCredentialStore.DockerCredentialHelper;

internal sealed class DockerCredentialHelperImplementation : IDockerCredentialHelper
{
  private readonly DockerCredentialHelperExecutable _credHelperExe;

  public DockerCredentialHelperImplementation(DockerCredentialHelperExecutable credentialHelperExe)
  {
    _credHelperExe = credentialHelperExe;
  }

  /// <inheritdoc/>
  public string ExecutableFilePath => _credHelperExe.ExecutableFilePath;

  /// <inheritdoc/>
  public string Version => DockerCredentialHelperExecutable.Version;

  /// <inheritdoc/>
  public async Task StoreAsync(Credentials credentials, CancellationToken cancellationToken)
  {
    var commandResult = await ExecuteCommandAsync(Command.Store, credentials.ToJson(), cancellationToken);
    if (commandResult.ExitCode != 0)
    {
      throw new CommandException(Command.Store, commandResult);
    }
  }

  /// <inheritdoc/>
  public async Task<Credentials> GetAsync(string serverURL, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(serverURL))
    {
      throw new ArgumentException($"{nameof(serverURL)} cannot be empty.");
    }

    var commandResult = await ExecuteCommandAsync(Command.Get, serverURL, cancellationToken);
    if (commandResult.ExitCode != 0)
    {
      throw new CommandException(Command.Get, commandResult);
    }

    return Credentials.GetCredentials(commandResult.Output);
  }

  /// <inheritdoc/>
  public async Task EraseAsync(string serverURL, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(serverURL))
    {
      throw new ArgumentException($"{nameof(serverURL)} cannot be empty.");
    }

    var commandResult = await ExecuteCommandAsync(Command.Erase, serverURL, cancellationToken);
    if (commandResult.ExitCode != 0)
    {
      // OSX keychain can throw an exception if the serverURL is not found
      // whereas windows credential doesn't. So to make it idempotent
      // on OSX we ignore error that has to do with "not found" message.
      // Similarly, "pass" returns "is not in the password store".
      if (IsCommandResultNotFound(commandResult))
      {
        return;
      }

      throw new CommandException(Command.Erase, commandResult);
    }
  }

  /// <inheritdoc/>
  public async Task<IDictionary<string, string>> ListAsync(CancellationToken cancellationToken)
  {
    var commandResult = await ExecuteCommandAsync(Command.List, cancellationToken: cancellationToken);
    if (commandResult.ExitCode != 0)
    {
      throw new CommandException(Command.List, commandResult);
    }

    var output = commandResult.Output;
    return JsonSerializer.Deserialize<IDictionary<string, string>>(output)!;
  }

  private async Task<CommandResult> ExecuteCommandAsync(
    Command command,
    string? arguments = null,
    CancellationToken cancellationToken = default
  )
  {
    var process = new Process()
    {
      StartInfo = new()
      {
        FileName = ExecutableFilePath,
        Arguments = command,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true,
        UseShellExecute = false
      }
    };

    if (!string.IsNullOrWhiteSpace(arguments))
    {
      process.StartInfo.RedirectStandardInput = true;
    }

    process.Start();

    // Write the argument to the stdin stream
    if (!string.IsNullOrWhiteSpace(arguments))
    {
      process.StandardInput.Write(arguments);
      process.StandardInput.Close();
    }

    await process.WaitForExitAsync(cancellationToken);

    var output = process.StandardOutput.ReadToEnd().Trim();
    var error = process.StandardError.ReadToEnd().Trim();
    return new CommandResult(process.ExitCode, output, error, ExecutableFilePath);
  }

  /// <summary>
  /// Return true if <paramref name="commandResult"/> indicates
  /// that the underlying credential service fails to find the
  /// given server URL.
  /// </summary>
  /// <param name="commandResult"></param>
  /// <returns>True if the command result is not found, false otherwise.</returns>
  /// <exception cref="NotSupportedException"></exception>
  private bool IsCommandResultNotFound(CommandResult commandResult)
  {
    var (_, output, error, _) = commandResult;
    var credService = _credHelperExe.CredentialService;

    if (credService == CredentialService.Keychain)
    {
      return HasNotFound("not found");
    }
    if (credService == CredentialService.Pass)
    {
      return HasNotFound("not in the password store");
    }

    throw new NotSupportedException($"Unsupported credential service {credService}.");

    bool HasNotFound(string text) =>
      output.Contains(text, StringComparison.OrdinalIgnoreCase) ||
      error.Contains(text, StringComparison.OrdinalIgnoreCase);
  }
}

using System.Text.Json;
using NativeCredentialStore.Platform;

namespace NativeCredentialStore;

internal sealed class NativeCredentialStore : INativeCredentialStore
{
  private readonly CredentialHelperExecutable _credHelperExe;

  public NativeCredentialStore(CredentialHelperExecutable credentialHelperExe)
  {
    _credHelperExe = credentialHelperExe;
  }

  public string ExecutableFilePath => _credHelperExe.ExecutableFilePath;

  public async Task StoreAsync(Credentials credentials, CancellationToken cancellationToken)
  {
    var commandResult = await ExecuteCommandAsync(Command.Store, credentials.ToJson(), cancellationToken);
    if (commandResult.ExitCode != 0)
    {
      throw new CommandException(Command.Store, commandResult);
    }
  }

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

  public async Task EraseAsync(string serverURL, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(serverURL))
    {
      throw new ArgumentException($"{nameof(serverURL)} cannot be empty.");
    }

    var commandResult = await ExecuteCommandAsync(Command.Erase, serverURL, cancellationToken);
    var (exitCode, output, error, _) = commandResult;

    if (exitCode != 0)
    {
      // OSX keychain throws an exception if the serverURL is not found
      // whereas windows credential doesn't. So to make it idempotent
      // on OSX we ignore error that has to do with "not found" message
      var isKeyChain = _credHelperExe.CredentialService == CredentialService.Keychain;
      var hasNotFoundMessage = output.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
                               error.Contains("not found", StringComparison.OrdinalIgnoreCase);

      if (isKeyChain && hasNotFoundMessage)
      {
        return;
      }
      throw new CommandException(Command.Erase, commandResult);
    }
  }

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
}

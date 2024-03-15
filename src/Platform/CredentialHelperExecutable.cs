using System.Reflection;

namespace NativeCredentialStore.Platform;

public sealed class CredentialHelperExecutable
{
  private static readonly string Version = string.Empty;

  private static readonly string ExecutableDirPath = string.Empty;

  static CredentialHelperExecutable()
  {
    var assembly = Assembly.GetAssembly(typeof(CredentialHelperExecutable));
    var dockerCredAttribute = assembly?.GetCustomAttribute<DockerCredentialHelperMetadataAttribute>();
    Version = dockerCredAttribute?.Version ?? 
      throw new InvalidOperationException("Could not get version from project.");

    var binPath = Path.GetDirectoryName(assembly?.Location)!;
    ExecutableDirPath = Path.Join(binPath, dockerCredAttribute.ToolsOutput);
  }

  public CredentialService CredentialService { get; }

  public OsPlatform OsPlatform { get; }

  public OsArchitecture OsArchitecture { get; }

  public string ExecutableName
  {
    get
    {
      var name = $"docker-credential-{CredentialService}-{Version}.{OsPlatform}-{OsArchitecture}";
      if (OsPlatform == OsPlatform.Windows)
      {
        name += ".exe";
      }
      return name;
    }
  }

  public string ExecutableFilePath 
    => Path.Join(ExecutableDirPath, ExecutableName);

  public CredentialHelperExecutable(
    CredentialService credentialService,
    OsPlatform osPlatform,
    OsArchitecture osArchitecture
  )
  {
    CredentialService = credentialService;
    OsPlatform = osPlatform;
    OsArchitecture = osArchitecture;

    if (!IsValid())
    {
      throw new ArgumentException("Unable to find executable given the combination of " +
                                  $"credential service ({CredentialService}), " +
                                  $"OS platform ({OsPlatform}), and OS architecture ({OsArchitecture}). " +
                                  $"Can't find executable file {ExecutableName}.");
    }
  }

  private bool IsValid()
  {
    var filePath = Path.Join(ExecutableDirPath, ExecutableName);
    return File.Exists(filePath);
  }
}

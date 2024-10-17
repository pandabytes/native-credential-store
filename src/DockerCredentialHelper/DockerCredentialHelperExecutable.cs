using System.Reflection;

namespace NativeCredentialStore.DockerCredentialHelper;

internal sealed class DockerCredentialHelperExecutable
{
  private static readonly string ExecutableDirPath;

  public static readonly string Version;

  static DockerCredentialHelperExecutable()
  {
    var assembly = Assembly.GetAssembly(typeof(DockerCredentialHelperExecutable));
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

  public DockerCredentialHelperExecutable(
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

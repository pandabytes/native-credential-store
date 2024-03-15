namespace NativeCredentialStore.Platform;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
internal sealed class DockerCredentialHelperMetadataAttribute : Attribute
{
  public string Version { get; }

  public string ToolsOutput { get; }

  public DockerCredentialHelperMetadataAttribute(string version, string toolsOutput)
  {
    Version = version;
    ToolsOutput = toolsOutput;
  }
}
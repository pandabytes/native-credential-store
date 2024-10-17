namespace NativeCredentialStore.DockerCredentialHelper;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
internal sealed class DockerCredentialHelperMetadataAttribute(string version, string toolsOutput) : Attribute
{
    public string Version { get; } = version;

    public string ToolsOutput { get; } = toolsOutput;
}

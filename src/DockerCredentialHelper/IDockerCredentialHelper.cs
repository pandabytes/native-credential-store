namespace NativeCredentialStore.DockerCredentialHelper;

/// <summary>
/// Interface to interact with the docker
/// credential helper executable. This mirrors
/// the interface https://pkg.go.dev/github.com/docker/docker-credential-helpers/client
/// </summary>
/// <remarks>
/// The <see cref="Credentials"/> attempts to
/// match with https://pkg.go.dev/github.com/docker/docker-credential-helpers@v0.8.1/credentials#Credentials.
/// </remarks>
public interface IDockerCredentialHelper : INativeCredentialStore
{
  /// <summary>
  /// Path to where the docker-credential-helper executable lives.
  /// This is the executable that will be called.
  /// </summary>
  string ExecutableFilePath { get; }

  /// <summary>
  /// The version of the docker-credential-helper.
  /// </summary>
  string Version { get; }
}

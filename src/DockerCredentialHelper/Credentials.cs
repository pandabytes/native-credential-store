using System.Text.Json;
using System.Text.Json.Serialization;

namespace NativeCredentialStore.DockerCredentialHelper;

/// <summary>
/// This attempts to match with
/// https://pkg.go.dev/github.com/docker/docker-credential-helpers@v0.8.2/credentials#Credentials.
/// </summary>
public sealed record Credentials
{
  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNamingPolicy = null,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
  };

  private readonly string _serverURL = string.Empty;

  private readonly string _username = string.Empty;

  private readonly string _secret = string.Empty;

  /// <summary>
  /// The server URL.
  /// </summary>
  public required string ServerURL
  {
    get => _serverURL;
    init
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException($"{nameof(ServerURL)} cannot be null or empty.");
      }
      _serverURL = value;
    }
  }

  /// <summary>
  /// The username.
  /// </summary>
  public required string Username
  {
    get => _username;
    init
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException($"{nameof(Username)} cannot be null or empty.");
      }
      _username = value;
    }
  }

  /// <summary>
  /// The secret or password.
  /// </summary>
  public required string Secret
  {
    get => _secret;
    init
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException($"{nameof(Secret)} cannot be null or empty.");
      }
      _secret = value;
    }
  }

  /// <summary>
  /// Return a JSON represantation of
  /// this object.
  /// </summary>
  /// <returns>JSON string.</returns>
  internal string ToJson() => JsonSerializer.Serialize(this, JsonOptions);

  internal static Credentials GetCredentials(string json)
  {
    var credentials = JsonSerializer.Deserialize<Credentials>(json, JsonOptions);
    return credentials ?? throw new ArgumentException(
      $"Failed to deserialize JSON to valid {typeof(Credentials).FullName}."
    );
  }
}

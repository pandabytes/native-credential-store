using System.Text.Json;
using System.Text.Json.Serialization;

namespace NativeCredentialStore;

public sealed class Credentials
{
  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNamingPolicy = null,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
  };

  private readonly string _serverURL = string.Empty;

  private readonly string _username = string.Empty;

  private readonly string _secret = string.Empty;

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

  public override string ToString()
    => $"{nameof(ServerURL)}={ServerURL};" +
       $"{nameof(Username)}={Username};" +
       $"{nameof(Secret)}={Secret};";

  public string ToJson() => JsonSerializer.Serialize(this, JsonOptions);

  public static Credentials GetCredentials(string json)
  {
    var credentials = JsonSerializer.Deserialize<Credentials>(json, JsonOptions);
    return credentials ?? throw new ArgumentException(
      $"Failed to deserialize JSON to valid {nameof(Credentials)}"
    );
  }
}

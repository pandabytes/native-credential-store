using Xunit.Abstractions;
using Xunit.Sdk;

namespace IntegrationTests.NativeCredentialStore;

public enum Platform
{
  All,
  Windows,
  MacOS,
  Linux,
}

[TraitDiscoverer("IntegrationTests.NativeCredentialStore.PlatformTraitDiscoverer", "IntegrationTests.NativeCredentialStore")]
[AttributeUsage(
  AttributeTargets.Method | AttributeTargets.Class,
  AllowMultiple = false
)]
public sealed class PlatformTraitAttribute : Attribute, ITraitAttribute
{
  public Platform Platform { get; }

  public PlatformTraitAttribute(Platform platform)
  {
    Platform = platform;
  }
}

public sealed class PlatformTraitDiscoverer : ITraitDiscoverer
{
    private const string Key = "Platform";

    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
      var platform = traitAttribute.GetNamedArgument<Platform>(Key);
      yield return new KeyValuePair<string, string>(Key, platform.ToString());
    }
}

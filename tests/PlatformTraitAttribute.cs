using Xunit.Abstractions;
using Xunit.Sdk;

namespace Tests.NativeCredentialStore;

public enum Platform
{
  All,
  Windows,
  MacOS,
  Linux,
}

[TraitDiscoverer("Tests.NativeCredentialStore.PlatformTraitDiscoverer", "Tests.NativeCredentialStore")]
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

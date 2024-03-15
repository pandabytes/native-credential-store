using Microsoft.Extensions.DependencyInjection;
using NativeCredentialStore.Platform;

namespace NativeCredentialStore;

public static class DependencyInjection
{
  public static IServiceCollection AddCredentialStore(
    this IServiceCollection services,
    OsArchitecture? osArchitecture = null,
    CredentialService? credentialService = null
  )
    => services.AddTransient(_ => CredentialStoreFactory.GetCredentialStore(osArchitecture, credentialService));
}

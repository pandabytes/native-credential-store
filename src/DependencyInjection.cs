using Microsoft.Extensions.DependencyInjection;

namespace NativeCredentialStore;

/// <summary>
/// Provide methods to inject dependencies.
/// </summary>
public static class DependencyInjection
{
  /// <summary>
  /// Inject <see cref="INativeCredentialStore"/> to DI
  /// container based on the device that this library
  /// runs on. Use the parameters for finer control.
  /// </summary>
  /// <param name="services">Services.</param>
  /// <param name="osArchitecture">Manually specify the OS architecture.</param>
  /// <param name="credentialService">Manually specify the credential service.</param>
  /// <exception cref="ArgumentException">
  /// Thrown when this methods fails to get an <see cref="INativeCredentialStore"/> object.
  /// </exception>
  public static IServiceCollection AddCredentialStore(
    this IServiceCollection services,
    OsArchitecture? osArchitecture = null,
    CredentialService? credentialService = null
  )
    => services.AddTransient(_ => CredentialStoreFactory.GetCredentialStore(osArchitecture, credentialService));
}

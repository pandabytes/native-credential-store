# Native Credential Store

This project is simply a wrapper of this project
[docker-credential-helpers](https://github.com/docker/docker-credential-helpers).
It provides similiar
[APIs](https://pkg.go.dev/github.com/docker/docker-credential-helpers/client?utm_source=godoc)
as the `docker-credential-helpers` does. 

All credit is due to the people who makes `docker-credential-helpers`.

This library aims to provide easy-to-use and cross-platform APIs for interacting
with native credential storage such as Windows Credential Manager or OSX keychain.
Dotnet does have a capability for storing/retreiving credential (or sensitive data)
called [Data Protection API](https://learn.microsoft.com/en-us/dotnet/standard/security/how-to-use-data-protection).
To my knowledge this API is only available on Windows, hence the motivation to
make this library cross-platform.

# Install
Install from [Nuget](https://www.nuget.org/packages/NativeCredentialStore).
```
dotnet add package NativeCredentialStore --version 0.0.0
```

# Usage
To access the native credential storage, you must have a
`INativeCredentialStore` object and there are 2 ways to get it:
1. Via the .NET Dependency Injection container - using `AddCredentialStore)()`
    ```cs
    using Microsoft.Extensions.DependencyInjection;
    using NativeCredentialStore;

    var serviceProvider = new ServiceCollection()
      .AddCredentialStore()
      .BuildServiceProvider();

    var credStoreService = serviceProvider.GetRequiredService<INativeCredentialStore>();
    ```
1. Via the factory class `CredentialStoreFactory.GetCredentialStore(...)`
    ```cs
    using static NativeCredentialStore.CredentialStoreFactory;

    var credStoreService = GetCredentialStore();
    ```

## Examples
```cs
using static NativeCredentialStore.CredentialStoreFactory;

var credStoreService = GetCredentialStore();

var credential = new Credential
{
  ServerURL = "http://nativecredentialstore.com",
  Username = "foo@email.com",
  Secret = "password"
};

// No return - can be used to update existing Credential
await credStoreService.StoreAsync(credential);

// Return the same Credential object we stored
var storedCredential = await credStoreService.GetAsync("http://nativecredentialstore.com");

// Return a dictionary looking like this:
// {"http://nativecredentialstore.com": "foo@email.com"}
var credentials = await credStoreService.ListAsync();

// Erase/remove the credential - this is idempontent
await credStoreService.EraseAsync("http://nativecredentialstore.com");

// Return an empty dictionary if there's no credential
credentials = await credStoreService.ListAsync();
```

# How it works?
During compilation, this project determines which executable files to download
based on the OS platform. Then during runtime, it calls those executable
to perform the commands (`get`, `erase`, `store`, and `list`).

The executables will be first downloaded to the project's `$(ToolsFolder)` path.
Then they are copied to the `bin` folder so that it is visible for the code to see. When a client
calls one of the methods such as `INativeCredentialStore.GetAsync(...)`, it will start a process to
call the appropriate executable and pass the arguments to that executable. The output of the
executable will be serialized to a C# object and it returns this object to the client.

When generating a nuget package, it will download all executables (for differnt OS's and platform architecture)
and pack all those files inside the nuget package (under `contentFiles` folder). When a consumer project
has a reference to the `NativeCredentialStore` library, all the executables will be copied to the `bin`
folder of the consumer project.

> [!NOTE]
>
> For now users on Mac AMD64 will have trouble running the executable because
> the `docker-credential-helpers` team has not fixed this signing this [issue](https://github.com/docker/docker-credential-helpers/issues/246#issuecomment-1690831962) yet.

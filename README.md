# Native Credential Store

This project is simply a wrapper of this project
[docker-credential-helpers](https://github.com/docker/docker-credential-helpers).
It provides similiar
[APIs](https://pkg.go.dev/github.com/docker/docker-credential-helpers/client?utm_source=godoc)
as the `docker-credential-helpers` does.

All credit is due to the people who makes `docker-credential-helpers`.

# How it works?
During compilation, this project determines which executable files to download
based on the OS platform. Then during runtime, it calls those executable
to perform the commands (`get`, `erase`, `store`, and `list`).

The executables will be first downloaded to the project's `tools/docker-credential-helper/v*/*`.
Then they are copied to the `bin` folder so that it is visible for the code to see. When a client
calls one of the methods such as `INativeCredentialStore.Get(...)`, it will start a process to
call the appropriate executable and pass the arguments to that executable. The output of the
executable will be serialized to a C# object and it returns this object to the client.

# Native Credential Store

This project is simply a wrapper of this project
[docker-credential-helpers](https://github.com/docker/docker-credential-helpers).
It provides similiar
[APIs](https://pkg.go.dev/github.com/docker/docker-credential-helpers/client?utm_source=godoc)
as the `docker-crednetial-helpers` does.

# How it works?
During compilation, this project determines which executable files to download
based on the OS platform. Then during runtime, it calls those executable
to perform the commands (`get`, `erase`, `store`, and `list`). These executables
are copied to the `bin` folder and also to the bin folder of the referenced
project.

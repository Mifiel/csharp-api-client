# csharp-api-client

C# SDK for the [Mifiel](https://www.mifiel.com) API.

## Documentation

API reference, guides, and examples:

- English: https://docs.mifiel.com/en/
- Español: https://docs.mifiel.com/es/

This README covers installation and client setup only.

## Installation

Requires **.NET 8** or later. The client is published on NuGet as [MifielAPIClient](https://www.nuget.org/packages/MifielAPIClient).

```shell
dotnet add package MifielAPIClient
```

Or from the Visual Studio Package Manager Console:

```shell
Install-Package MifielAPIClient
```

## Setup

1. Create an account (production or [sandbox](https://app-sandbox.mifiel.com)).
2. Generate an `APP_ID` and `APP_SECRET` in [Access Tokens](https://app-sandbox.mifiel.com/settings/access-tokens).
3. Configure the client:

```csharp
using MifielAPI;

ApiClient apiClient = new ApiClient(appId, appSecret);
// Production is the default (https://app.mifiel.com).
// For sandbox:
apiClient.Url = "https://app-sandbox.mifiel.com";
```

## Releasing

This SDK ships as the NuGet package **MifielAPIClient** ([nuget.org/packages/MifielAPIClient](https://www.nuget.org/packages/MifielAPIClient)). It targets `net8.0` and is built with the .NET SDK (`dotnet pack` / `dotnet nuget push`).

1. **Bump the version** in `MifielAPI/MifielAPI/MifielAPI.csproj` (`<Version>`) and add a heading in `CHANGELOG.md`.
2. **Pack:**

   ```shell
   dotnet pack MifielAPI/MifielAPI/MifielAPI.csproj -c Release -o artifacts
   ```

3. **Publish to nuget.org** with an API key from [nuget.org/account/apikeys](https://www.nuget.org/account/apikeys):

   ```shell
   dotnet nuget push artifacts/MifielAPIClient.<version>.nupkg \
     --source https://api.nuget.org/v3/index.json \
     --api-key "$NUGET_API_KEY"
   ```

4. **Tag the git commit** and create a GitHub release.

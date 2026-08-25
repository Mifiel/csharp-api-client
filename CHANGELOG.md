# Changelog

## v1.0.0 - 2026-08-21

### Breaking changes

- Default API host changed from `https://www.mifiel.com` to `https://app.mifiel.com`.
- Sandbox documentation and examples now use `https://app-sandbox.mifiel.com` instead of `https://sandbox.mifiel.com`.
- Package version jumped from `0.0.4` to `1.0.0` to mark this default-host breaking change.

### Features

- Send a standardized `User-Agent` on API requests, e.g. `DOTNET/4.0.30319.42000 MifielAPIClient/1.0.0 HttpClient/4.0.0.0 (Unix/6.8.0)`.

### Migration

```csharp
using MifielAPI;

ApiClient apiClient = new ApiClient(appId, appSecret);
// Production requests now go to https://app.mifiel.com/api/v1/...

// Sandbox
apiClient.Url = "https://app-sandbox.mifiel.com";
```

If you previously set a legacy host explicitly, update it or remove the override to use the new default.

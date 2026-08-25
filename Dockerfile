# Build and run the NUnit smoke test with the .NET SDK (this library targets net8.0).
# Usage:
#   docker build -t csharp-api-client-smoke .
#   docker run --rm csharp-api-client-smoke
# Optional env: MIFIEL_APP_ID, MIFIEL_APP_SECRET, MIFIEL_BASE_URL
FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src
COPY . /src

RUN dotnet restore MifielAPI/MifielAPI.sln \
    && dotnet build MifielAPI/MifielAPI.sln --configuration Release --no-restore

WORKDIR /src
CMD ["dotnet", "test", "MifielAPI/MifielAPI.sln", "--configuration", "Release", "--no-build", "--filter", "Category=Smoke"]

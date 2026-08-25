# Build and run tests with the .NET SDK (this library targets net8.0).
# Usage:
#   docker build -t csharp-api-client-tests .
#   docker run --rm csharp-api-client-tests
FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src
COPY . /src

RUN dotnet restore MifielAPI/MifielAPI.sln \
    && dotnet build MifielAPI/MifielAPI.sln --configuration Release --no-restore \
    && chown -R app:app /src

USER app
CMD ["dotnet", "test", "MifielAPI/MifielAPI.sln", "--configuration", "Release", "--no-build", "--filter", "Category!=Smoke"]

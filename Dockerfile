# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY WebBanThuocBVTV/WebBanThuocBVTV.csproj WebBanThuocBVTV/
RUN dotnet restore WebBanThuocBVTV/WebBanThuocBVTV.csproj

COPY . .
RUN dotnet publish WebBanThuocBVTV/WebBanThuocBVTV.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render containers have a low inotify watcher limit. Config files do not need
# hot reload in production, so disable it before ASP.NET builds configuration.
ENV ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false \
    DOTNET_USE_POLLING_FILE_WATCHER=1
EXPOSE 10000

# Render provides PORT at runtime (10000 by default).
ENTRYPOINT ["sh", "-c", "dotnet WebBanThuocBVTV.dll --urls http://0.0.0.0:${PORT:-10000}"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./src ./src
COPY ./AiFeatureFlagsPlatform.sln ./

RUN dotnet publish ./src/AiFeatureFlags.Api/AiFeatureFlags.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ConnectionStrings__Sqlite="Data Source=/data/app.db;Foreign Keys=True"
ENV Jwt__Issuer="AiFeatureFlagsPlatform"
ENV Jwt__Audience="AiFeatureFlagsPlatformClients"
ENV Jwt__SigningKey="docker-demo-signing-key-at-least-32-bytes!!"
ENV Jwt__AccessTokenMinutes="120"
ENV SeedDemoData="true"

VOLUME ["/data"]

ENTRYPOINT ["dotnet", "AiFeatureFlags.Api.dll"]

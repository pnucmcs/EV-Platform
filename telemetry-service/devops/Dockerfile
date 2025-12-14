FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TelemetryService.sln .
COPY Api/Ev.Telemetry.Api.csproj Api/
COPY Application/Ev.Telemetry.Application.csproj Application/
COPY Domain/Ev.Telemetry.Domain.csproj Domain/
COPY Infrastructure/Ev.Telemetry.Infrastructure.csproj Infrastructure/
COPY shared/Ev.Platform.Contracts/Ev.Platform.Contracts.csproj shared/Ev.Platform.Contracts/
COPY shared/Ev.Shared.Messaging/Ev.Shared.Messaging.csproj shared/Ev.Shared.Messaging/

RUN dotnet restore TelemetryService.sln

COPY . .
RUN dotnet publish Api/Ev.Telemetry.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Ev.Telemetry.Api.dll"]

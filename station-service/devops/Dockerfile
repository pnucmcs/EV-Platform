FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY StationService.sln .
COPY Api/Ev.Station.Api/Ev.Station.Api.csproj Api/Ev.Station.Api/
COPY Application/Ev.Station.Application/Ev.Station.Application.csproj Application/Ev.Station.Application/
COPY Domain/Ev.Station.Domain/Ev.Station.Domain.csproj Domain/Ev.Station.Domain/
COPY Infrastructure/Ev.Station.Infrastructure/Ev.Station.Infrastructure.csproj Infrastructure/Ev.Station.Infrastructure/
COPY shared/Ev.Platform.Contracts/Ev.Platform.Contracts.csproj shared/Ev.Platform.Contracts/
COPY shared/Ev.Shared.Messaging/Ev.Shared.Messaging.csproj shared/Ev.Shared.Messaging/

RUN dotnet restore StationService.sln

COPY . .
RUN dotnet publish Api/Ev.Station.Api/Ev.Station.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Ev.Station.Api.dll"]

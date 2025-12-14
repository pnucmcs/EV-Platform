FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ReportingService.sln .
COPY Api/Ev.Reporting.Api.csproj Api/
COPY Application/Ev.Reporting.Application.csproj Application/
COPY Domain/Ev.Reporting.Domain.csproj Domain/
COPY Infrastructure/Ev.Reporting.Infrastructure.csproj Infrastructure/
COPY shared/Ev.Platform.Contracts/Ev.Platform.Contracts.csproj shared/Ev.Platform.Contracts/
COPY shared/Ev.Shared.Messaging/Ev.Shared.Messaging.csproj shared/Ev.Shared.Messaging/

RUN dotnet restore ReportingService.sln

COPY . .
RUN dotnet publish Api/Ev.Reporting.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Ev.Reporting.Api.dll"]

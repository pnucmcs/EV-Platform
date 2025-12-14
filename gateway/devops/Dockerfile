FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Gateway.sln .
COPY Api/Ev.Gateway.Api.csproj Api/
COPY Application/Ev.Gateway.Application.csproj Application/
COPY Domain/Ev.Gateway.Domain.csproj Domain/
COPY Infrastructure/Ev.Gateway.Infrastructure.csproj Infrastructure/
COPY shared/Ev.Platform.Contracts/Ev.Platform.Contracts.csproj shared/Ev.Platform.Contracts/
COPY shared/Ev.Shared.Messaging/Ev.Shared.Messaging.csproj shared/Ev.Shared.Messaging/

RUN dotnet restore Gateway.sln

COPY . .
RUN dotnet publish Api/Ev.Gateway.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Ev.Gateway.Api.dll"]

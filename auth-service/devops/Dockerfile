FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY AuthService.sln .
COPY Api/Ev.Auth.Api/Ev.Auth.Api.csproj Api/Ev.Auth.Api/
COPY Application/Ev.Auth.Application/Ev.Auth.Application.csproj Application/Ev.Auth.Application/
COPY Domain/Ev.Auth.Domain/Ev.Auth.Domain.csproj Domain/Ev.Auth.Domain/
COPY Infrastructure/Ev.Auth.Infrastructure/Ev.Auth.Infrastructure.csproj Infrastructure/Ev.Auth.Infrastructure/
COPY shared/Ev.Platform.Contracts/Ev.Platform.Contracts.csproj shared/Ev.Platform.Contracts/
COPY shared/Ev.Shared.Messaging/Ev.Shared.Messaging.csproj shared/Ev.Shared.Messaging/

RUN dotnet restore AuthService.sln

COPY . .
RUN dotnet publish Api/Ev.Auth.Api/Ev.Auth.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Ev.Auth.Api.dll"]

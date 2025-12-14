FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY NotificationService.sln .
COPY Api/Ev.Notification.Api.csproj Api/
COPY Application/Ev.Notification.Application.csproj Application/
COPY Domain/Ev.Notification.Domain.csproj Domain/
COPY Infrastructure/Ev.Notification.Infrastructure.csproj Infrastructure/
COPY shared/Ev.Platform.Contracts/Ev.Platform.Contracts.csproj shared/Ev.Platform.Contracts/
COPY shared/Ev.Shared.Messaging/Ev.Shared.Messaging.csproj shared/Ev.Shared.Messaging/

RUN dotnet restore NotificationService.sln

COPY . .
RUN dotnet publish Api/Ev.Notification.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Ev.Notification.Api.dll"]

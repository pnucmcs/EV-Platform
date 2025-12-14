FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files for caching
COPY BillingService.sln .
COPY Api/Ev.Billing.Api.csproj Api/
COPY Application/Ev.Billing.Application.csproj Application/
COPY Domain/Ev.Billing.Domain.csproj Domain/
COPY Infrastructure/Ev.Billing.Infrastructure.csproj Infrastructure/
COPY shared/Ev.Platform.Contracts/Ev.Platform.Contracts.csproj shared/Ev.Platform.Contracts/
COPY shared/Ev.Shared.Messaging/Ev.Shared.Messaging.csproj shared/Ev.Shared.Messaging/

RUN dotnet restore BillingService.sln

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish Api/Ev.Billing.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Ev.Billing.Api.dll"]

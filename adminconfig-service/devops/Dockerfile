FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first to leverage Docker layer caching.
COPY AdminconfigService.sln .
COPY Api/Ev.AdminConfig.Api.csproj Api/
COPY Application/Ev.AdminConfig.Application.csproj Application/
COPY Domain/Ev.AdminConfig.Domain.csproj Domain/
COPY Infrastructure/Ev.AdminConfig.Infrastructure.csproj Infrastructure/

RUN dotnet restore AdminconfigService.sln

# Copy the remaining source and publish the application.
COPY . .
RUN dotnet publish Api/Ev.AdminConfig.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Ev.AdminConfig.Api.dll"]

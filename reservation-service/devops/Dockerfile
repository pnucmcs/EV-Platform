FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ReservationService.sln .
COPY Api/Ev.Reservation.Api/Ev.Reservation.Api.csproj Api/Ev.Reservation.Api/
COPY Application/Ev.Reservation.Application/Ev.Reservation.Application.csproj Application/Ev.Reservation.Application/
COPY Domain/Ev.Reservation.Domain/Ev.Reservation.Domain.csproj Domain/Ev.Reservation.Domain/
COPY Infrastructure/Ev.Reservation.Infrastructure/Ev.Reservation.Infrastructure.csproj Infrastructure/Ev.Reservation.Infrastructure/
COPY shared/Ev.Platform.Contracts/Ev.Platform.Contracts.csproj shared/Ev.Platform.Contracts/
COPY shared/Ev.Shared.Messaging/Ev.Shared.Messaging.csproj shared/Ev.Shared.Messaging/

RUN dotnet restore ReservationService.sln

COPY . .
RUN dotnet publish Api/Ev.Reservation.Api/Ev.Reservation.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Ev.Reservation.Api.dll"]

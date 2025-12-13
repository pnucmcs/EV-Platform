# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
WORKDIR /src
COPY . .
RUN dotnet restore src/services/reservation-service/Api/Ev.Reservation.Api/Ev.Reservation.Api.csproj

FROM restore AS build
RUN dotnet build src/services/reservation-service/Api/Ev.Reservation.Api/Ev.Reservation.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish src/services/reservation-service/Api/Ev.Reservation.Api/Ev.Reservation.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ev.Reservation.Api.dll"]

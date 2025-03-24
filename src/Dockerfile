FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY . .
WORKDIR "/src/PV260.API.App"

RUN dotnet restore "PV260.API.App.csproj"
RUN dotnet build "PV260.API.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PV260.API.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PV260.API.App.dll"]

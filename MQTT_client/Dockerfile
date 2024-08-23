FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MQTT_client/MQTT_client.csproj", "MQTT_client/"]
COPY ["DATABASE_library/DATABASE_library.csproj", "DATABASE_library/"]
RUN dotnet restore "MQTT_client/MQTT_client.csproj"
COPY . .
WORKDIR "/src/MQTT_client"
RUN dotnet build "MQTT_client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MQTT_client.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "mqtt_client.dll"]

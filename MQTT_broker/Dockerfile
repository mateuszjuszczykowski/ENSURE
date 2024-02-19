FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MQTT_broker/MQTT_broker.csproj", "MQTT_broker/"]
RUN dotnet restore "MQTT_broker/MQTT_broker.csproj"
COPY . .
WORKDIR "/src/MQTT_broker"
RUN dotnet build "MQTT_broker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MQTT_broker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MQTT_broker.dll"]

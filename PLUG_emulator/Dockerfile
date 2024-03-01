FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PLUG_emulator/PLUG_emulator.csproj", "PLUG_emulator/"]
RUN dotnet restore "PLUG_emulator/PLUG_emulator.csproj"
COPY . .
WORKDIR "/src/PLUG_emulator"
RUN dotnet build "PLUG_emulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PLUG_emulator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PLUG_emulator.dll"]

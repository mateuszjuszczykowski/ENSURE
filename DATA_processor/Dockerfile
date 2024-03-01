FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DATA_processor/DATA_processor.csproj", "DATA_processor/"]
RUN dotnet restore "DATA_processor/DATA_processor.csproj"
COPY . .
WORKDIR "/src/DATA_processor"
RUN dotnet build "DATA_processor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DATA_processor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DATA_processor.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DATA_viewer/DATA_viewer.csproj", "DATA_viewer/"]
RUN dotnet restore "DATA_viewer/DATA_viewer.csproj"
COPY . .
WORKDIR "/src/DATA_viewer"
RUN dotnet build "DATA_viewer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DATA_viewer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DATA_viewer.dll"]

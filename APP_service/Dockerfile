FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["APP_service/APP_service.csproj", "APP_service/"]
RUN dotnet restore "APP_service/APP_service.csproj"
COPY . .
WORKDIR "/src/APP_service"
RUN dotnet build "APP_service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APP_service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APP_service.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["INCOMING_service/INCOMING_service.csproj", "INCOMING_service/"]
RUN dotnet restore "INCOMING_service/INCOMING_service.csproj"
COPY . .
WORKDIR "/src/INCOMING_service"
RUN dotnet build "INCOMING_service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "INCOMING_service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "INCOMING_service.dll"]

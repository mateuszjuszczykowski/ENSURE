FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["IDENTITY_service/IDENTITY_service.csproj", "IDENTITY_service/"]
RUN dotnet restore "IDENTITY_service/IDENTITY_service.csproj"
COPY . .
WORKDIR "/src/IDENTITY_service"
RUN dotnet build "IDENTITY_service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IDENTITY_service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IDENTITY_service.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ["CatalogingSystem.Api/CatalogingSystem.Api.csproj", "CatalogingSystem.Api/"]
COPY ["CatalogingSystem.Core/CatalogingSystem.Core.csproj", "CatalogingSystem.Core/"]
COPY ["CatalogingSystem.Data/CatalogingSystem.Data.csproj", "CatalogingSystem.Data/"]
COPY ["CatalogingSystem.DTOs/CatalogingSystem.DTOs.csproj", "CatalogingSystem.DTOs/"]
COPY ["CatalogingSystem.Services/CatalogingSystem.Services.csproj", "CatalogingSystem.Services/"]

RUN dotnet restore "CatalogingSystem.Api/CatalogingSystem.Api.csproj"
COPY . .
WORKDIR "/app/CatalogingSystem.Api"
RUN dotnet build "CatalogingSystem.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CatalogingSystem.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogingSystem.Api.dll"]
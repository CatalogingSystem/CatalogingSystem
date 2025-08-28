# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar csproj y restaurar dependencias
COPY *.sln .
COPY CatalogingSystem.Api/*.csproj ./CatalogingSystem.Api/
COPY CatalogingSystem.Core/*.csproj ./CatalogingSystem.Core/
COPY CatalogingSystem.Data/*.csproj ./CatalogingSystem.Data/
COPY CatalogingSystem.DTOs/*.csproj ./CatalogingSystem.DTOs/
COPY CatalogingSystem.Services/*.csproj ./CatalogingSystem.Services/
RUN dotnet restore

# Copiar todo el código y build
COPY . .
WORKDIR /app/CatalogingSystem.Api
RUN dotnet publish -c Release -o out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/CatalogingSystem.Api/out ./
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "CatalogingSystem.Api.dll"]
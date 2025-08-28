# Etapa 1: Compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copiar los archivos .csproj y restaurar dependencias
COPY *.sln .
COPY CatalogingSystem.Api/*.csproj ./CatalogingSystem.Api/
COPY CatalogingSystem.Core/*.csproj ./CatalogingSystem.Core/
COPY CatalogingSystem.Data/*.csproj ./CatalogingSystem.Data/
COPY CatalogingSystem.DTOs/*.csproj ./CatalogingSystem.DTOs/
COPY CatalogingSystem.Services/*.csproj ./CatalogingSystem.Services/
RUN dotnet restore

# Copiar todo el código fuente y compilar la aplicación
COPY . .
WORKDIR /source/CatalogingSystem.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Etapa 2: Ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Railway proporciona la variable de entorno PORT.
# ASP.NET Core 8 la usará automáticamente.
# El Entrypoint inicia tu API.
ENTRYPOINT ["dotnet", "CatalogingSystem.Api.dll"]
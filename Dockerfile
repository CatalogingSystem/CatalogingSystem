# Etapa 1: Compilación y Migraciones (Usa la imagen del SDK completa)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Instalar la herramienta de Entity Framework Core de forma global
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copiar los archivos .csproj y restaurar dependencias
COPY *.sln .
COPY CatalogingSystem.Api/*.csproj ./CatalogingSystem.Api/
COPY CatalogingSystem.Core/*.csproj ./CatalogingSystem.Core/
COPY CatalogingSystem.Data/*.csproj ./CatalogingSystem.Data/
COPY CatalogingSystem.DTOs/*.csproj ./CatalogingSystem.DTOs/
COPY CatalogingSystem.Services/*.csproj ./CatalogingSystem.Services/
RUN dotnet restore

# Copiar todo el código fuente
COPY . .

# ---> ¡AQUÍ ESTÁ LA MAGIA! <---
# Ejecutar las migraciones ANTES de publicar la aplicación.
# Nota cómo los paths son relativos a la raíz del proyecto, donde está el .sln
RUN dotnet ef database update --project CatalogingSystem.Data --startup-project CatalogingSystem.Api --context BaseDbContext
RUN dotnet ef database update --project CatalogingSystem.Data --startup-project CatalogingSystem.Api --context ApplicationDbContext

# Publicar la aplicación para la etapa final
WORKDIR /source/CatalogingSystem.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Etapa 2: Ejecución (Usa la imagen ligera de ASP.NET Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CatalogingSystem.Api.dll"]
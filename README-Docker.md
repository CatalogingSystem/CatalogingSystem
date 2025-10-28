# CatalogingSystem Backend - Ejecución Independiente

## 🚀 Ejecución con Docker

```bash
docker-compose up --build
```

Esto iniciará:

- **API**: `http://localhost:8080`
- **PostgreSQL**: `localhost:5432`
- **pgAdmin**: `http://localhost:5050`

## 🔧 Configuración

### Variables de Entorno

Asegúrate de que el archivo `.env` en `CatalogingSystem.Api/` esté configurado correctamente.

### CORS

El backend está configurado para aceptar peticiones desde:

- `http://localhost:5173` (desarrollo con Vite)
- `http://localhost:3001` (contenedor frontend)
- `http://localhost:3000` (backup puerto)

## 🔍 Verificación

### API Funcionando

- Swagger: `http://localhost:8080/swagger`
- Health Check: `http://localhost:8080/health` (si está configurado)

### Base de Datos

- pgAdmin: `http://localhost:5050`
- Credenciales por defecto:
  - Email: `CatalogingSystem@CatalogingSystem.com`
  - Password: `CatalogingSystem`

## 🛠️ Comandos Útiles

```bash
# Ver logs
docker-compose logs -f api

# Detener servicios
docker-compose down

# Resetear base de datos
docker-compose down -v
docker-compose up --build

# Solo API (sin pgAdmin)
docker-compose up api postgres
```

## 🐛 Solución de Problemas

### Puerto 8080 ocupado

Cambia el puerto en `docker-compose.yml`:

```yaml
services:
  api:
    ports:
      - "8081:8080" # Cambia a otro puerto
```

### Problemas con migraciones

```bash
# Resetear completamente
docker-compose down -v
docker volume prune
docker-compose up --build
```

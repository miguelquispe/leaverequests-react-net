# Leave Requests App

Sistema de gestión de solicitudes de permisos con backend en .NET y frontend en React.


## 🏛️ Decisiones de Diseño

### Backend (.NET)

- **Arquitectura por Capas**: Separación clara entre Domain, Application, Infrastructure y Controllers para mejor mantenibilidad y testabilidad
- **Clean Architecture**: Dependencias apuntan hacia el dominio, facilitando cambios sin impacto en la lógica de negocio
- **FluentValidation**: Validación declarativa y reutilizable separada de los DTOs para mejor organización del código
- **Entity Framework**: ORM para abstracción de base de datos con Code-First approach, configurado con InMemory provider para desarrollo rápido y testing sin dependencias externas
- **API Response Wrapper**: Formato consistente de respuestas con manejo centralizado de errores y metadatos
- **Dependency Injection**: Inversión de control nativa de .NET para bajo acoplamiento y facilidad de testing

#### Endpoints API
- **GET** `/api/leaverequests` - Obtener solicitudes (employee ve las suyas; manager ve todas)
- **POST** `/api/leaverequests` - Crear nueva solicitud
- **PUT** `/api/leaverequests/{id}` - Actualizar estado (solo manager)
- **DELETE** `/api/leaverequests/{id}` - Cancelar solicitud (solo el propietario si está pendiente)

#### Business Rules
- No se permiten permisos aprobados superpuestos para el mismo empleado

> **Nota para Testing**: El usuario y rol se pasan mediante HTTP headers (`UserId`, `UserRole`) para facilitar pruebas. **TODO**: Mejorar la integración de autenticación y autorización para producción.

### Frontend (React)

- **Feature-Based Architecture**: Organización por funcionalidades (`/features/leaverequests`) en lugar de por tipo de archivo para mejor escalabilidad
- **React Query**: Manejo de estado del servidor con cache automático, invalidación inteligente y sincronización de datos
- **Context API**: Estado global para autenticación y navegación, evitando prop drilling innecesario
- **Custom Hooks**: Encapsulación de lógica de negocio reutilizable (useAuth, useLeaveRequestForm) para componentes más limpios  
- **TypeScript**: Type safety completo con generación automática de tipos desde la API para consistencia backend-frontend
- **OpenAPI Code Generation**: Generación automática de tipos TypeScript desde Swagger (`NODE_TLS_REJECT_UNAUTHORIZED=0 npm run generate-api`) para sincronización automática de contratos API
- **Componentes Composables**: Separación entre componentes de presentación y lógica de negocio para mayor reutilización



## 📋 Prerequisitos

- **Backend**: .NET 8 SDK o Visual Studio 2022
- **Frontend**: Node.js 18+ y npm
- **Database**: No requerida (usa InMemory EF)

## 🚀 Instalación y Ejecución

### Backend (.NET API)

```bash
# Opción 1: Línea de comandos
cd backend/LeaveRequestAPI
dotnet restore
dotnet run  # InMemory DB se inicializa automáticamente

# Opción 2: Visual Studio 2022 (recomendado)
# Abrir backend/LeaveRequestAPI.sln
# Establecer LeaveRequestAPI como proyecto de inicio
# F5 para ejecutar
```

**API disponible en:** `https://localhost:7076`
**Swagger UI:** `https://localhost:7076/swagger`

### Frontend (React)

```bash
# Navegar al directorio frontend
cd frontend

# Instalar dependencias
npm install

# Ejecutar en desarrollo
npm run dev
```

**Frontend disponible en:** `http://localhost:5173`

## 🧪 Testing

```bash
# Backend - Ejecutar tests
cd backend/LeaveRequestAPI.Tests
dotnet test

# Backend - Desde Visual Studio 2022
# 1. Agregar proyecto de tests a la solución:
#    Click derecho en Solution > Add > Existing Project
#    Seleccionar: LeaveRequestAPI.Tests/LeaveRequestAPI.Tests.csproj
# 2. Test > Run All Tests (Ctrl+R, A)
# 3. O usar Test Explorer (Test > Test Explorer)

# Frontend - Ejecutar tests
cd frontend
npm test

# Frontend - Tests en modo watch
npm run test:watch
```

## 🔧 Comandos Útiles

### Backend
- **Visual Studio 2022**: F5 para ejecutar (recomendado)
- **Agregar Tests**: Add > Existing Project > LeaveRequestAPI.Tests.csproj
- **Test Explorer**: Ctrl+R, A para ejecutar tests (VS 2022)
- `dotnet run` - Ejecutar API (CLI)
- `dotnet test` - Ejecutar tests (CLI)
- `dotnet build` - Compilar proyecto

### Frontend
- `npm run dev` - Desarrollo
- `npm run build` - Build producción
- `npm run lint` - Linting código
- `NODE_TLS_REJECT_UNAUTHORIZED=0 npm run generate-api` - Generar tipos desde Swagger (desarrollo)

## 🏗️ Estructura del Proyecto

```
├── backend/                # API .NET
│   ├── LeaveRequestAPI/    # Proyecto principal
│   └── LeaveRequestAPI.Tests/
├── frontend/               # React app
│   ├── src/
│   │   ├── features/       # Features por módulo
│   │   ├── contexts/       # React contexts
│   │   └── api/           # Cliente API
└── README.md
```

## 🔑 Funcionalidades

- **Empleados**: Crear solicitudes de permisos
- **Managers**: Aprobar/rechazar solicitudes
- **Dashboard**: Vista general de solicitudes

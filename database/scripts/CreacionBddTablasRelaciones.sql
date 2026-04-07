CREATE DATABASE BitacoraVehicularDB;
GO

USE BitacoraVehicularDB;
GO

-- =========================================
-- TABLAS MAESTRAS
-- =========================================

CREATE TABLE Roles (
    RolId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE ContextosEstado (
    ContextoEstadoId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Estados (
    EstadoId INT IDENTITY(1,1) PRIMARY KEY,
    ContextoEstadoId INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Estados_ContextosEstado
        FOREIGN KEY (ContextoEstadoId) REFERENCES ContextosEstado(ContextoEstadoId)
);

CREATE TABLE Marcas (
    MarcaId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE Modelos (
    ModeloId INT IDENTITY(1,1) PRIMARY KEY,
    MarcaId INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Modelos_Marcas
        FOREIGN KEY (MarcaId) REFERENCES Marcas(MarcaId)
);

CREATE TABLE TiposCombustible (
    TipoCombustibleId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE TiposMantencion (
    TipoMantencionId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
);

-- =========================================
-- TABLAS OPERACIONALES
-- =========================================

CREATE TABLE Usuarios (
    UsuarioId INT IDENTITY(1,1) PRIMARY KEY,
    RolId INT NOT NULL,
    EstadoId INT NOT NULL,
    NombreCompleto NVARCHAR(150) NOT NULL,
    Correo NVARCHAR(150) NOT NULL UNIQUE,
    NombreUsuario NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaActualizacion DATETIME NULL,
    CONSTRAINT FK_Usuarios_Roles
        FOREIGN KEY (RolId) REFERENCES Roles(RolId),
    CONSTRAINT FK_Usuarios_Estados
        FOREIGN KEY (EstadoId) REFERENCES Estados(EstadoId)
);

CREATE TABLE Vehiculos (
    VehiculoId INT IDENTITY(1,1) PRIMARY KEY,
    ModeloId INT NOT NULL,
    EstadoId INT NOT NULL,
    Patente NVARCHAR(20) NOT NULL UNIQUE,
    Anio INT NOT NULL,
    Color NVARCHAR(50) NOT NULL,
    KilometrajeActual INT NOT NULL DEFAULT 0,
    Observacion NVARCHAR(255) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaActualizacion DATETIME NULL,
    CONSTRAINT FK_Vehiculos_Modelos
        FOREIGN KEY (ModeloId) REFERENCES Modelos(ModeloId),
    CONSTRAINT FK_Vehiculos_Estados
        FOREIGN KEY (EstadoId) REFERENCES Estados(EstadoId)
);

CREATE TABLE AsignacionesVehiculo (
    AsignacionVehiculoId INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,
    VehiculoId INT NOT NULL,
    EstadoId INT NOT NULL,
    FechaAsignacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaTermino DATETIME NULL,
    Observacion NVARCHAR(255) NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Asignaciones_Usuarios
        FOREIGN KEY (UsuarioId) REFERENCES Usuarios(UsuarioId),
    CONSTRAINT FK_Asignaciones_Vehiculos
        FOREIGN KEY (VehiculoId) REFERENCES Vehiculos(VehiculoId),
    CONSTRAINT FK_Asignaciones_Estados
        FOREIGN KEY (EstadoId) REFERENCES Estados(EstadoId)
);

CREATE TABLE Viajes (
    ViajeId INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,
    VehiculoId INT NOT NULL,
    EstadoId INT NOT NULL,
    FechaHoraSalida DATETIME NOT NULL DEFAULT GETDATE(),
    FechaHoraLlegada DATETIME NULL,
    KilometrajeSalida INT NOT NULL,
    KilometrajeLlegada INT NULL,
    ObservacionSalida NVARCHAR(255) NULL,
    ObservacionLlegada NVARCHAR(255) NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaActualizacion DATETIME NULL,
    CONSTRAINT FK_Viajes_Usuarios
        FOREIGN KEY (UsuarioId) REFERENCES Usuarios(UsuarioId),
    CONSTRAINT FK_Viajes_Vehiculos
        FOREIGN KEY (VehiculoId) REFERENCES Vehiculos(VehiculoId),
    CONSTRAINT FK_Viajes_Estados
        FOREIGN KEY (EstadoId) REFERENCES Estados(EstadoId)
);

CREATE TABLE CargasCombustible (
    CargaCombustibleId INT IDENTITY(1,1) PRIMARY KEY,
    ViajeId INT NOT NULL,
    UsuarioId INT NOT NULL,
    VehiculoId INT NOT NULL,
    TipoCombustibleId INT NOT NULL,
    Litros DECIMAL(10,2) NULL,
    MontoCargado DECIMAL(12,2) NOT NULL,
    FechaCarga DATETIME NOT NULL DEFAULT GETDATE(),
    NombreArchivoBoleta NVARCHAR(255) NULL,
    RutaArchivoBoleta NVARCHAR(500) NULL,
    Observacion NVARCHAR(255) NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Cargas_Viajes
        FOREIGN KEY (ViajeId) REFERENCES Viajes(ViajeId),
    CONSTRAINT FK_Cargas_Usuarios
        FOREIGN KEY (UsuarioId) REFERENCES Usuarios(UsuarioId),
    CONSTRAINT FK_Cargas_Vehiculos
        FOREIGN KEY (VehiculoId) REFERENCES Vehiculos(VehiculoId),
    CONSTRAINT FK_Cargas_TipoCombustible
        FOREIGN KEY (TipoCombustibleId) REFERENCES TiposCombustible(TipoCombustibleId)
);

CREATE TABLE Mantenciones (
    MantencionId INT IDENTITY(1,1) PRIMARY KEY,
    VehiculoId INT NOT NULL,
    TipoMantencionId INT NOT NULL,
    EstadoId INT NOT NULL,
    FechaIngreso DATETIME NOT NULL DEFAULT GETDATE(),
    FechaSalida DATETIME NULL,
    KilometrajeVehiculo INT NOT NULL,
    Detalle NVARCHAR(500) NOT NULL,
    Costo DECIMAL(12,2) NULL,
    ProximaMantencionKm INT NULL,
    ProximaMantencionFecha DATETIME NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaActualizacion DATETIME NULL,
    CONSTRAINT FK_Mantenciones_Vehiculos
        FOREIGN KEY (VehiculoId) REFERENCES Vehiculos(VehiculoId),
    CONSTRAINT FK_Mantenciones_TipoMantencion
        FOREIGN KEY (TipoMantencionId) REFERENCES TiposMantencion(TipoMantencionId),
    CONSTRAINT FK_Mantenciones_Estados
        FOREIGN KEY (EstadoId) REFERENCES Estados(EstadoId)
);
GO

-- =========================================
-- ÍNDICES DE REGLAS DE NEGOCIO
-- =========================================

-- Un usuario no puede tener más de un viaje abierto
CREATE UNIQUE INDEX UX_Viajes_Usuario_Abierto
ON Viajes (UsuarioId)
WHERE FechaHoraLlegada IS NULL;
GO

-- Un vehículo no puede tener más de un viaje abierto
CREATE UNIQUE INDEX UX_Viajes_Vehiculo_Abierto
ON Viajes (VehiculoId)
WHERE FechaHoraLlegada IS NULL;
GO
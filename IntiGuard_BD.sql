-- Crear base de datos
CREATE DATABASE IntiGuardDB;
GO

-- Usar base de datos
USE IntiGuardDB;
GO

-- Tabla Rol
CREATE TABLE rol (
    id_rol INT PRIMARY KEY IDENTITY(1,1),
    nombre_rol VARCHAR(50) NOT NULL
);
GO

-- Tabla Usuario
CREATE TABLE usuario (
    id_usuario INT PRIMARY KEY IDENTITY(1,1),
    nombres VARCHAR(100) NOT NULL,
    apellidos VARCHAR(100) NOT NULL,
    correo VARCHAR(100) NOT NULL UNIQUE,
    clave VARCHAR(255),
    id_rol INT NOT NULL,
    fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (id_rol) REFERENCES rol(id_rol)
);
GO

-- Tabla Producto
CREATE TABLE producto (
    id_producto INT PRIMARY KEY IDENTITY(1,1),
    nombre_producto VARCHAR(150) NOT NULL,
    descripcion NVARCHAR(MAX),
    marca VARCHAR(50),
    precio DECIMAL(10, 2) NOT NULL,
    stock INT NOT NULL,
    imagen_url VARCHAR(500)
);
GO

-- Tabla Comprobante
CREATE TABLE comprobante (
    id_comprobante INT PRIMARY KEY IDENTITY(1,1),
    tipo_comprobante VARCHAR(50) NOT NULL,
    numero_comprobante VARCHAR(20) NOT NULL UNIQUE
);
GO

-- Tabla Venta
CREATE TABLE venta (
    id_venta INT PRIMARY KEY IDENTITY(1,1),
    id_usuario INT NOT NULL,
    id_comprobante INT NOT NULL,
    total DECIMAL(10, 2) NOT NULL,
    fecha_venta DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario),
    FOREIGN KEY (id_comprobante) REFERENCES comprobante(id_comprobante)
);
GO

-- Tabla DetalleVenta
CREATE TABLE detalle_venta (
    id_detalle_venta INT PRIMARY KEY IDENTITY(1,1),
    id_venta INT NOT NULL,
    id_producto INT NOT NULL,
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (id_venta) REFERENCES venta(id_venta),
    FOREIGN KEY (id_producto) REFERENCES producto(id_producto)
);
GO
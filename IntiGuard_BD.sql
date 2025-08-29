/* *
*   Estructura IntiGuard
*   Base de datos y tablas.
*/

CREATE DATABASE IntiGuardDB;
GO

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
    telefono VARCHAR(20),
    direccion VARCHAR(100),
    foto TEXT,
    clave VARCHAR(255),
    id_rol INT NOT NULL,
    fecha_registro DATETIME NOT NULL DEFAULT GETDATE(),
    activo bit not null default 1,
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

/* *
*   Store procedures
*   Procedimientos almacenados que usa el sistema
*/

USE IntiGuardDB;
GO

-- ========================================
-- CRUD USUARIO
-- ========================================

-- Crear usuario
CREATE PROCEDURE sp_usuario_create
    @nombres VARCHAR(100),
    @apellidos VARCHAR(100),
    @correo VARCHAR(100),
    @telefono VARCHAR(20),
    @direccion VARCHAR(100),
    @foto TEXT,
    @clave VARCHAR(255),
    @id_rol INT
AS
BEGIN
    INSERT INTO usuario (nombres, apellidos, correo,telefono,direccion,foto ,clave, id_rol)
    VALUES (@nombres, @apellidos, @correo,@telefono,@direccion,@foto ,@clave, @id_rol);
END
GO

-- Listar todos los usuarios
CREATE PROCEDURE sp_usuario_get_all
AS
BEGIN
    SELECT u.id_usuario, u.nombres, u.apellidos, u.correo,u.telefono,u.direccion,u.foto ,u.id_rol, r.nombre_rol, u.fecha_registro, u.activo
    FROM usuario u
    INNER JOIN rol r ON u.id_rol = r.id_rol;
END
GO

-- Obtener usuario por ID
CREATE PROCEDURE sp_usuario_get_by_id
    @id_usuario INT
AS
BEGIN
    SELECT u.id_usuario, u.nombres, u.apellidos, u.correo, u.telefono,u.direccion,u.foto, u.id_rol, r.nombre_rol, u.fecha_registro, u.activo
    FROM usuario u
    INNER JOIN rol r ON u.id_rol = r.id_rol
    WHERE u.id_usuario = @id_usuario;
END
GO

-- Actualizar usuario
CREATE PROCEDURE sp_usuario_update
    @id_usuario INT,
    @nombres VARCHAR(100),
    @apellidos VARCHAR(100),
    @correo VARCHAR(100),
    @telefono VARCHAR(20),
    @direccion VARCHAR(100),
    @foto TEXT,
    @clave VARCHAR(255),
    @id_rol INT
AS
BEGIN
    UPDATE usuario
    SET nombres = @nombres,
        apellidos = @apellidos,
        correo = @correo,
        telefono = @telefono,
        direccion = @direccion,
        foto = @foto,
        clave = @clave,
        id_rol = @id_rol
    WHERE id_usuario = @id_usuario;
END
GO

-- Eliminar usuario
CREATE PROCEDURE sp_usuario_delete
    @id_usuario INT
AS
BEGIN
    DELETE FROM usuario WHERE id_usuario = @id_usuario;
END
GO

-- ========================================
-- CRUD PRODUCTO
-- ========================================

-- Crear producto
CREATE PROCEDURE sp_producto_create
    @nombre_producto VARCHAR(150),
    @descripcion NVARCHAR(MAX),
    @marca VARCHAR(50),
    @precio DECIMAL(10,2),
    @stock INT,
    @imagen_url VARCHAR(500)
AS
BEGIN
    INSERT INTO producto (nombre_producto, descripcion, marca, precio, stock, imagen_url)
    VALUES (@nombre_producto, @descripcion, @marca, @precio, @stock, @imagen_url);
END
GO

-- Listar todos los productos
CREATE PROCEDURE sp_producto_get_all
AS
BEGIN
    SELECT * FROM producto;
END
GO

-- Obtener producto por ID
CREATE PROCEDURE sp_producto_get_by_id
    @id_producto INT
AS
BEGIN
    SELECT * FROM producto WHERE id_producto = @id_producto;
END
GO

-- Actualizar producto
CREATE PROCEDURE sp_producto_update
    @id_producto INT,
    @nombre_producto VARCHAR(150),
    @descripcion NVARCHAR(MAX),
    @marca VARCHAR(50),
    @precio DECIMAL(10,2),
    @stock INT,
    @imagen_url VARCHAR(500)
AS
BEGIN
    UPDATE producto
    SET nombre_producto = @nombre_producto,
        descripcion = @descripcion,
        marca = @marca,
        precio = @precio,
        stock = @stock,
        imagen_url = @imagen_url
    WHERE id_producto = @id_producto;
END
GO

-- Eliminar producto
CREATE PROCEDURE sp_producto_delete
    @id_producto INT
AS
BEGIN
    DELETE FROM producto WHERE id_producto = @id_producto;
END
GO

-- Descontar el stock
CREATE PROCEDURE sp_producto_descontar_stock_transaccion
    @id_producto INT,
    @cantidad INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE producto
    SET stock = stock - @cantidad
    WHERE id_producto = @id_producto;
END
GO

-- ========================================
-- CONSULTAS ROL
-- ========================================

-- Listar roles
CREATE PROCEDURE sp_rol_get_all
AS
BEGIN
    SELECT * FROM rol;
END
GO

-- ========================================
-- CONSULTAS COMPROBANTE
-- ========================================

-- Listar comprobantes
CREATE PROCEDURE sp_comprobante_get_all
AS
BEGIN
    SELECT * FROM comprobante;
END
GO

-- ========================================
-- CONSULTAS VENTA
-- ========================================

-- Listar ventas
CREATE PROCEDURE sp_venta_get_all
AS
BEGIN
    SELECT v.id_venta, u.nombres + ' ' + u.apellidos AS cliente, c.tipo_comprobante,
           c.numero_comprobante, v.total, v.fecha_venta
    FROM venta v
    INNER JOIN usuario u ON v.id_usuario = u.id_usuario
    INNER JOIN comprobante c ON v.id_comprobante = c.id_comprobante;
END
GO

-- Obtener venta por id
CREATE PROCEDURE sp_venta_get_by_id
    @id_venta INT
AS
BEGIN
    SELECT v.id_venta, u.nombres + ' ' + u.apellidos AS cliente, c.tipo_comprobante,
           c.numero_comprobante, v.total, v.fecha_venta
    FROM venta v
    INNER JOIN usuario u ON v.id_usuario = u.id_usuario
    INNER JOIN comprobante c ON v.id_comprobante = c.id_comprobante
    WHERE v.id_venta = @id_venta;
END
GO

-- Crear venta inicial
CREATE PROCEDURE sp_venta_create_transaccion
    @id_usuario INT,
    @id_comprobante INT,
    @total DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO venta (id_usuario, id_comprobante, total)
    VALUES (@id_usuario, @id_comprobante, @total);
    SELECT SCOPE_IDENTITY() AS id_venta;
END
GO

CREATE PROCEDURE sp_venta_delete_by_usuario
    @id_venta INT,
    @id_usuario INT
AS
BEGIN
    -- Validar si la venta pertenece al usuario
    IF EXISTS (
        SELECT 1 FROM venta
        WHERE id_venta = @id_venta AND id_usuario = @id_usuario
    )
    BEGIN
	DELETE FROM detalle_venta 
	WHERE id_venta = @id_venta;

        DELETE FROM venta
        WHERE id_venta = @id_venta;
    END
    ELSE
    BEGIN
        -- Lanzar error si no existe o no pertenece al usuario
        RAISERROR('La venta no existe o no pertenece al usuario especificado.', 16, 1);
    END
END
GO


-- ========================================
-- CONSULTAS DETALLE VENTA
-- ========================================

-- Listar detalles de la venta
CREATE PROCEDURE sp_detalle_venta_get_by_venta
    @id_venta INT
AS
BEGIN
    SELECT dv.id_detalle_venta, p.nombre_producto, dv.cantidad, dv.precio_unitario
    FROM detalle_venta dv
    INNER JOIN producto p ON dv.id_producto = p.id_producto
    WHERE dv.id_venta = @id_venta;
END
GO

-- Insertar detalles inicial
CREATE PROCEDURE sp_detalle_venta_create_transaccion
    @id_venta INT,
    @id_producto INT,
    @cantidad INT,
    @precio_unitario DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO detalle_venta (id_venta, id_producto, cantidad, precio_unitario)
    VALUES (@id_venta, @id_producto, @cantidad, @precio_unitario);
END
GO

/* *
*   Data inicial
*   Datos iniciales para el arranque del sistema
*/

USE IntiGuardDB
GO

-- Insertar roles
INSERT INTO rol (nombre_rol) VALUES ('ADMIN');
INSERT INTO rol (nombre_rol) VALUES ('USER');
GO

-- Insertar usuarios ( Contraseña hasheada = contra1234)
INSERT INTO usuario (nombres, apellidos, correo,telefono,direccion,foto,clave,id_rol) 
VALUES 
('Gabriel', 'Sanchez', 'gabriel.sanchez@gmail.com','999888763','Matuzalen','https://thumbs.dreamstime.com/b/vector-de-perfil-avatar-predeterminado-foto-usuario-redes-sociales-desconocida-icono-desconocido-en-184816085.jpg' ,'$2a$12$qyB3d6SXESXK6SeZcgQvC.Uxm/GiqcxCzo8UK1cUS/xQUdPjYx4c2', 2),
('Cesar', 'Mendoza', 'cesar.mendoza@gmail.com','910658333','Av los cipreces','https://thumbs.dreamstime.com/b/vector-de-perfil-avatar-predeterminado-foto-usuario-redes-sociales-desconocida-icono-desconocido-en-184816085.jpg' ,'$2a$12$qyB3d6SXESXK6SeZcgQvC.Uxm/GiqcxCzo8UK1cUS/xQUdPjYx4c2', 2),
('Diego', 'Vega', 'diego.vega@gmail.com','987654321','Av pueblo paleta','https://thumbs.dreamstime.com/b/vector-de-perfil-avatar-predeterminado-foto-usuario-redes-sociales-desconocida-icono-desconocido-en-184816085.jpg','$2a$12$qyB3d6SXESXK6SeZcgQvC.Uxm/GiqcxCzo8UK1cUS/xQUdPjYx4c2', 2),
('Hector', 'Gonzalez', 'hector.gonzalez@gmail.com','910658335','Los Angeles','https://thumbs.dreamstime.com/b/vector-de-perfil-avatar-predeterminado-foto-usuario-redes-sociales-desconocida-icono-desconocido-en-184816085.jpg','$2a$12$qyB3d6SXESXK6SeZcgQvC.Uxm/GiqcxCzo8UK1cUS/xQUdPjYx4c2', 1),
('Erick', 'Castillo', 'erick.castillo@gmail.com','999888333','Av Imposible','https://thumbs.dreamstime.com/b/vector-de-perfil-avatar-predeterminado-foto-usuario-redes-sociales-desconocida-icono-desconocido-en-184816085.jpg', '$2a$12$qyB3d6SXESXK6SeZcgQvC.Uxm/GiqcxCzo8UK1cUS/xQUdPjYx4c2', 1);
GO

-- Insertar productos
INSERT INTO producto (nombre_producto, descripcion, marca, precio, stock, imagen_url) 
VALUES 
('Camara de Seguridad IP', 'Camara de seguridad con visi n nocturna, 4K y Wi-Fi', 'Dahua', 250.00, 100, 'https://example.com/camara_ip.jpg'),
('Alarma Inteligente', 'Sistema de alarma con sensores de movimiento y conexi n a app', 'Honeywell', 180.00, 80, 'https://example.com/alarma_inteligente.jpg'),
('Cerradura Digital', 'Cerradura inteligente para puertas con control remoto', 'Samsung', 350.00, 150, 'https://example.com/cerradura_digital.jpg'),
('Detectores de Humo', 'Detector de humo para sistemas de alarma de incendio', 'Nest', 120.00, 200, 'https://example.com/detector_humo.jpg'),
('Camara de Seguridad Fija', 'Camara fija de seguridad con resoluci n HD', 'Sony', 180.00, 75, 'https://example.com/camara_fija.jpg'),
('C mara PTZ', 'C mara con movimiento pan-tilt-zoom controlable a distancia', 'Bosch', 500.00, 50, 'https://example.com/camara_ptz.jpg'),
('Sistema de Videovigilancia', 'Kit de videovigilancia de 8 c maras con grabador', 'Vivotek', 1000.00, 30, 'https://example.com/videovigilancia.jpg'),
('Intercomunicador', 'Intercomunicador de video para control de accesos', '2N', 400.00, 60, 'https://example.com/intercomunicador.jpg'),
('Sensor de Movimiento', 'Sensor de movimiento para sistemas de alarma', 'Hikvision', 90.00, 150, 'https://example.com/sensor_movimiento.jpg'),
('C mara Domo', 'C mara de seguridad tipo domo para interiores y exteriores', 'Axis', 350.00, 120, 'https://example.com/camara_domo.jpg'),
('Caja Fuerte Electr nica', 'Caja fuerte con cerradura electr nica para protecci n de documentos', 'SentrySafe', 220.00, 50, 'https://example.com/caja_fuerte.jpg'),
('Control de Acceso RFID', 'Sistema de control de acceso por tarjetas RFID', 'Schneider', 450.00, 40, 'https://example.com/control_acceso.jpg'),
('C mara de Seguridad para Exterior', 'C mara de seguridad con resistencia a la intemperie', 'Arlo', 300.00, 100, 'https://example.com/camara_exterior.jpg'),
('Bot n de P nico', 'Bot n de p nico para alertas de emergencia', 'Vesta', 80.00, 200, 'https://example.com/boton_panico.jpg'),
('Sistema de Alarma para Veh culos', 'Alarma para veh culos con sensores y control remoto', 'Viper', 150.00, 80, 'https://example.com/alarma_vehiculo.jpg');
GO

-- Insertar comprobantes
INSERT INTO comprobante (tipo_comprobante, numero_comprobante)
VALUES 
('Factura', 'F0001'),
('Boleta', 'B0001'),
('Factura', 'F0002'),
('Boleta', 'B0002'),
('Factura', 'F0003');
GO

-- Insertar ventas
INSERT INTO venta (id_usuario, id_comprobante, total) 
VALUES 
(1, 1, 1200.00),
(2, 2, 700.00),
(3, 3, 2500.00),
(4, 4, 1200.00),
(5, 5, 800.00);
GO

-- Insertar detalles de ventas
INSERT INTO detalle_venta (id_venta, id_producto, cantidad, precio_unitario)
VALUES 
(1, 1, 1, 1200.00),
(2, 2, 1, 700.00),
(3, 3, 3, 800.00),
(4, 4, 4, 250.00),
(5, 5, 2, 400.00);
GO

USE IntiGuardDB;
GO

UPDATE producto SET 
    nombre_producto = 'Cámara de Seguridad IP',
    descripcion = 'Cámara de seguridad con visión nocturna, 4K y Wi-Fi',
    marca = 'Dahua',
    imagen_url = '/assets/img/portfolio/1.jpg'
WHERE id_producto = 1;

UPDATE producto SET 
    nombre_producto = 'Alarma Inteligente',
    descripcion = 'Sistema de alarma con sensores de movimiento y conexión a app',
    marca = 'Honeywell',
    imagen_url = '/assets/img/portfolio/2.jpg'
WHERE id_producto = 2;

UPDATE producto SET 
    nombre_producto = 'Cerradura Digital',
    descripcion = 'Cerradura inteligente para puertas con control remoto',
    marca = 'Samsung',
    imagen_url = '/assets/img/portfolio/3.jpg'
WHERE id_producto = 3;

UPDATE producto SET 
    nombre_producto = 'Detectores de Humo',
    descripcion = 'Detector de humo para sistemas de alarma de incendio',
    marca = 'Nest',
    imagen_url = '/assets/img/portfolio/4.jpg'
WHERE id_producto = 4;

UPDATE producto SET 
    nombre_producto = 'Cámara de Seguridad Fija',
    descripcion = 'Cámara fija de seguridad con resolución HD',
    marca = 'Sony',
    imagen_url = '/assets/img/portfolio/5.jpg'
WHERE id_producto = 5;

UPDATE producto SET 
    nombre_producto = 'Cámara PTZ',
    descripcion = 'Cámara con movimiento pan-tilt-zoom controlable a distancia',
    marca = 'Bosch',
    imagen_url = '/assets/img/portfolio/6.jpg'
WHERE id_producto = 6;

UPDATE producto SET 
    nombre_producto = 'Sistema de Videovigilancia',
    descripcion = 'Kit de videovigilancia de 8 cámaras con grabador',
    marca = 'Vivotek',
    imagen_url = '/assets/img/portfolio/7.jpg'
WHERE id_producto = 7;

UPDATE producto SET 
    nombre_producto = 'Intercomunicador',
    descripcion = 'Intercomunicador de video para control de accesos',
    marca = '2N',
    imagen_url = '/assets/img/portfolio/8.jpg'
WHERE id_producto = 8;

UPDATE producto SET 
    nombre_producto = 'Sensor de Movimiento',
    descripcion = 'Sensor de movimiento para sistemas de alarma',
    marca = 'Hikvision',
    imagen_url = '/assets/img/portfolio/9.jpg'
WHERE id_producto = 9;

UPDATE producto SET 
    nombre_producto = 'Cámara Domo',
    descripcion = 'Cámara de seguridad tipo domo para interiores y exteriores',
    marca = 'Axis',
    imagen_url = '/assets/img/portfolio/10.jpg'
WHERE id_producto = 10;

UPDATE producto SET 
    nombre_producto = 'Caja Fuerte Electrónica',
    descripcion = 'Caja fuerte con cerradura electrónica para protección de documentos',
    marca = 'SentrySafe',
    imagen_url = '/assets/img/portfolio/11.jpg'
WHERE id_producto = 11;

UPDATE producto SET 
    nombre_producto = 'Control de Acceso RFID',
    descripcion = 'Sistema de control de acceso por tarjetas RFID',
    marca = 'Schneider',
    imagen_url = '/assets/img/portfolio/12.jpg'
WHERE id_producto = 12;

UPDATE producto SET 
    nombre_producto = 'Cámara de Seguridad para Exterior',
    descripcion = 'Cámara de seguridad con resistencia a la intemperie',
    marca = 'Arlo',
    imagen_url = '/assets/img/portfolio/13r.jpg'
WHERE id_producto = 13;

UPDATE producto SET 
    nombre_producto = 'Botón de Pánico',
    descripcion = 'Botón de pánico para alertas de emergencia',
    marca = 'Vesta',
    imagen_url = '/assets/img/portfolio/14.jpg'
WHERE id_producto = 14;

UPDATE producto SET 
    nombre_producto = 'Sistema de Alarma para Vehículos',
    descripcion = 'Alarma para vehículos con sensores y control remoto',
    marca = 'Viper',
    imagen_url = '/assets/img/portfolio/15.jpg'
WHERE id_producto = 15;
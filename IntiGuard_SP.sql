-- ========================================
-- Procedimientos almacenados IntiGuardDB
-- ========================================
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
    @clave VARCHAR(255),
    @id_rol INT
AS
BEGIN
    INSERT INTO usuario (nombres, apellidos, correo,telefono,direccion ,clave, id_rol)
    VALUES (@nombres, @apellidos, @correo,@telefono,@direccion ,@clave, @id_rol);
END
GO

-- Listar todos los usuarios
CREATE PROCEDURE sp_usuario_get_all
AS
BEGIN
    SELECT u.id_usuario, u.nombres, u.apellidos, u.correo,u.telefono,u.direccion ,u.id_rol, r.nombre_rol, u.fecha_registro
    FROM usuario u
    INNER JOIN rol r ON u.id_rol = r.id_rol;
END
GO

-- Obtener usuario por ID
CREATE PROCEDURE sp_usuario_get_by_id
    @id_usuario INT
AS
BEGIN
    SELECT u.id_usuario, u.nombres, u.apellidos, u.correo, u.telefono,u.direccion, u.id_rol, r.nombre_rol, u.fecha_registro
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

-- ========================================
-- PROCEDIMIENTOS ALMACENADOS
-- ========================================

USE IntiGuardDB;
GO

-- Leer todos los usuarios con su rol
CREATE OR ALTER PROCEDURE sp_leer_usuarios AS
BEGIN
    SELECT u.id_usuario, u.nombres, u.apellidos, u.correo, u.fecha_registro, r.id_rol, r.nombre_rol
    FROM usuario u
    INNER JOIN rol r ON u.id_rol = r.id_rol;
END;
GO

-- Validar usuario por correo
CREATE OR ALTER PROCEDURE sp_validar_usuario(@correo VARCHAR(100)) AS
BEGIN
    SELECT id_usuario, nombres, apellidos, correo, clave, id_rol
    FROM usuario
    WHERE correo = @correo;
END;
GO

-- Leer usuario por ID
CREATE OR ALTER PROCEDURE sp_leer_usuario_por_id(@id_usuario INT) AS
BEGIN
    SELECT id_usuario, nombres, apellidos, correo, fecha_registro, id_rol
    FROM usuario
    WHERE id_usuario = @id_usuario;
END;
GO

-- Actualizar datos del usuario
CREATE OR ALTER PROCEDURE sp_actualizar_usuario(
    @id_usuario INT,
    @nombres VARCHAR(100),
    @apellidos VARCHAR(100),
    @correo VARCHAR(100),
    @id_rol INT
) AS
BEGIN
    UPDATE usuario
    SET nombres = @nombres,
        apellidos = @apellidos,
        correo = @correo,
        id_rol = @id_rol
    WHERE id_usuario = @id_usuario;
END;
GO

-- Cambiar contraseña del usuario
CREATE OR ALTER PROCEDURE sp_cambiar_clave_usuario(
    @id_usuario INT,
    @nueva_clave VARCHAR(255)
) AS
BEGIN
    UPDATE usuario
    SET clave = @nueva_clave
    WHERE id_usuario = @id_usuario;
END;
GO

-- Leer producto por ID
CREATE OR ALTER PROCEDURE sp_leer_producto_por_id(@id_producto INT) AS
BEGIN
    SELECT * FROM producto WHERE id_producto = @id_producto;
END;
GO

-- Buscar productos por nombre o marca
CREATE OR ALTER PROCEDURE sp_buscar_productos(@termino_busqueda VARCHAR(150)) AS
BEGIN
    SELECT * FROM producto
    WHERE nombre_producto LIKE '%' + @termino_busqueda + '%'
        OR marca LIKE '%' + @termino_busqueda + '%';
END;
GO

-- Actualizar stock de un producto
CREATE OR ALTER PROCEDURE sp_actualizar_stock_producto(
    @id_producto INT,
    @cantidad_vendida INT
) AS
BEGIN
    UPDATE producto
    SET stock = stock - @cantidad_vendida
    WHERE id_producto = @id_producto;
END;
GO

-- Crear venta y detalle de venta
CREATE OR ALTER PROCEDURE sp_crear_venta(
    @id_usuario INT,
    @id_comprobante INT,
    @total DECIMAL(10, 2),
    @detalles dbo.DetalleVentaType READONLY,
    @id_venta_generado INT OUTPUT
) AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO venta (id_usuario, id_comprobante, total)
        VALUES (@id_usuario, @id_comprobante, @total);

        SET @id_venta_generado = SCOPE_IDENTITY();

        INSERT INTO detalle_venta (id_venta, id_producto, cantidad, precio_unitario)
        SELECT @id_venta_generado, id_producto, cantidad, precio_unitario FROM @detalles;

        DECLARE @id_prod INT, @cantidad INT;
        DECLARE cur CURSOR FOR SELECT id_producto, cantidad FROM @detalles;
        OPEN cur;
        FETCH NEXT FROM cur INTO @id_prod, @cantidad;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            EXEC sp_actualizar_stock_producto @id_producto = @id_prod, @cantidad_vendida = @cantidad;
            FETCH NEXT FROM cur INTO @id_prod, @cantidad;
        END
        CLOSE cur;
        DEALLOCATE cur;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- Registrar nuevo usuario (cliente)
CREATE OR ALTER PROCEDURE sp_registrar_usuario (
    @nombres VARCHAR(100),
    @apellidos VARCHAR(100),
    @correo VARCHAR(100),
    @clave VARCHAR(255)
) AS
BEGIN
    INSERT INTO usuario (nombres, apellidos, correo, clave, id_rol)
    VALUES (@nombres, @apellidos, @correo, @clave, 2); -- Asume que 2 = Cliente
END;
GO

-- Listar productos más vendidos
CREATE OR ALTER PROCEDURE sp_listar_productos_mas_vendidos (
    @top_n INT
) AS
BEGIN
    SELECT TOP (@top_n)
        p.nombre_producto,
        SUM(dv.cantidad) AS total_vendido
    FROM detalle_venta dv
    JOIN producto p ON dv.id_producto = p.id_producto
    GROUP BY p.nombre_producto
    ORDER BY total_vendido DESC;
END;
GO

-- Reporte de ventas por usuario
CREATE OR ALTER PROCEDURE sp_reporte_ventas_por_usuario AS
BEGIN
    SELECT
        u.id_usuario,
        u.nombres + ' ' + u.apellidos AS cliente,
        COUNT(v.id_venta) AS total_ventas,
        SUM(v.total) AS monto_total
    FROM usuario u
    JOIN venta v ON u.id_usuario = v.id_usuario
    GROUP BY u.id_usuario, u.nombres, u.apellidos
    ORDER BY monto_total DESC;
END;
GO

-- Listar productos con stock bajo
CREATE OR ALTER PROCEDURE sp_listar_productos_con_stock_bajo (
    @stock_minimo INT
) AS
BEGIN
    SELECT id_producto, nombre_producto, stock
    FROM producto
    WHERE stock <= @stock_minimo;
END;
GO

-- Historial de compras por usuario
CREATE OR ALTER PROCEDURE sp_listar_historial_compras_usuario (
    @id_usuario INT
) AS
BEGIN
    SELECT
        v.id_venta,
        v.fecha_venta,
        c.tipo_comprobante,
        c.numero_comprobante,
        p.nombre_producto,
        dv.cantidad,
        dv.precio_unitario,
        (dv.cantidad * dv.precio_unitario) AS subtotal_calculated
    FROM venta v
    JOIN detalle_venta dv ON v.id_venta = dv.id_venta
    JOIN producto p ON dv.id_producto = p.id_producto
    JOIN comprobante c ON v.id_comprobante = c.id_comprobante
    WHERE v.id_usuario = @id_usuario
    ORDER BY v.fecha_venta DESC;
END;
GO
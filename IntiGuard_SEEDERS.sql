USE IntiGuardDB
GO

-- Insertar roles
INSERT INTO rol (nombre_rol) VALUES ('ADMIN');
INSERT INTO rol (nombre_rol) VALUES ('USER');
GO

-- Insertar usuarios
INSERT INTO usuario (nombres, apellidos, correo,telefono,direccion,foto,clave,id_rol) 
VALUES 
('Gabriel', 'S�nchez', 'gabriel.sanchez@gmail.com','999888763','Matuzalen','https://thumbs.dreamstime.com/b/vector-de-perfil-avatar-predeterminado-foto-usuario-redes-sociales-desconocida-icono-desconocido-en-184816085.jpg' ,'clave123', 2),
('C�sar', 'Mendoza', 'cesar.mendoza@gmail.com','910658333','Av los cipreces','https://thumbs.dreamstime.com/b/vector-de-perfil-avatar-predeterminado-foto-usuario-redes-sociales-desconocida-icono-desconocido-en-184816085.jpg' ,'clave456', 2),
('Diego', 'Vega', 'diego.vega@gmail.com','987654321','Av pueblo paleta','https://thumbs.dreamstime.com/b/vector-de-perfil-avatar-predeterminado-foto-usuario-redes-sociales-desconocida-icono-desconocido-en-184816085.jpg','clave789', 2),
('H�ctor', 'Gonz�lez', 'hector.gonzalez@gmail.com','910658335','Los Angeles','https://thumbs.dreamstime.com/b/vector-de-perfil-avatar-predeterminado-foto-usuario-redes-sociales-desconocida-icono-desconocido-en-184816085.jpg','admin1234', 1),
('Erick', 'Castillo', 'erick.castillo@gmail.com','999888333','Av Imposible','https://thumbs.dreamstime.com/b/vector-de-perfil-avatar-predeterminado-foto-usuario-redes-sociales-desconocida-icono-desconocido-en-184816085.jpg', 'admin5678', 1);
GO

-- Insertar productos
INSERT INTO producto (nombre_producto, descripcion, marca, precio, stock, imagen_url) 
VALUES 
('Camara de Seguridad IP', 'Camara de seguridad con visi�n nocturna, 4K y Wi-Fi', 'Dahua', 250.00, 100, 'https://example.com/camara_ip.jpg'),
('Alarma Inteligente', 'Sistema de alarma con sensores de movimiento y conexi�n a app', 'Honeywell', 180.00, 80, 'https://example.com/alarma_inteligente.jpg'),
('Cerradura Digital', 'Cerradura inteligente para puertas con control remoto', 'Samsung', 350.00, 150, 'https://example.com/cerradura_digital.jpg'),
('Detectores de Humo', 'Detector de humo para sistemas de alarma de incendio', 'Nest', 120.00, 200, 'https://example.com/detector_humo.jpg'),
('Camara de Seguridad Fija', 'Camara fija de seguridad con resoluci�n HD', 'Sony', 180.00, 75, 'https://example.com/camara_fija.jpg'),
('C�mara PTZ', 'C�mara con movimiento pan-tilt-zoom controlable a distancia', 'Bosch', 500.00, 50, 'https://example.com/camara_ptz.jpg'),
('Sistema de Videovigilancia', 'Kit de videovigilancia de 8 c�maras con grabador', 'Vivotek', 1000.00, 30, 'https://example.com/videovigilancia.jpg'),
('Intercomunicador', 'Intercomunicador de video para control de accesos', '2N', 400.00, 60, 'https://example.com/intercomunicador.jpg'),
('Sensor de Movimiento', 'Sensor de movimiento para sistemas de alarma', 'Hikvision', 90.00, 150, 'https://example.com/sensor_movimiento.jpg'),
('C�mara Domo', 'C�mara de seguridad tipo domo para interiores y exteriores', 'Axis', 350.00, 120, 'https://example.com/camara_domo.jpg'),
('Caja Fuerte Electr�nica', 'Caja fuerte con cerradura electr�nica para protecci�n de documentos', 'SentrySafe', 220.00, 50, 'https://example.com/caja_fuerte.jpg'),
('Control de Acceso RFID', 'Sistema de control de acceso por tarjetas RFID', 'Schneider', 450.00, 40, 'https://example.com/control_acceso.jpg'),
('C�mara de Seguridad para Exterior', 'C�mara de seguridad con resistencia a la intemperie', 'Arlo', 300.00, 100, 'https://example.com/camara_exterior.jpg'),
('Bot�n de P�nico', 'Bot�n de p�nico para alertas de emergencia', 'Vesta', 80.00, 200, 'https://example.com/boton_panico.jpg'),
('Sistema de Alarma para Veh�culos', 'Alarma para veh�culos con sensores y control remoto', 'Viper', 150.00, 80, 'https://example.com/alarma_vehiculo.jpg');
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
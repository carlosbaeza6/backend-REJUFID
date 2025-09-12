CREATE TABLE Acciones_usuario (
    Id_accion INT IDENTITY(1,1) PRIMARY KEY,
    Id_usuario VARCHAR(100) NOT NULL,
    Nombre_completo VARCHAR(100),
    Matricula VARCHAR(50),
    Fecha_accion DATETIME DEFAULT GETDATE(),
    Modulo_afectado VARCHAR(100),
    Tipo_accion VARCHAR(50),
    Descripcion TEXT
);
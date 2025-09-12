CREATE TABLE Login_history (
    Id_login INT IDENTITY(1,1) PRIMARY KEY,
    Id_usuario VARCHAR(100) NOT NULL,
    Nombre_completo VARCHAR(100),
    Matricula VARCHAR(50),
    Fecha_ingreso DATETIME DEFAULT GETDATE(),
    Direccion_ip VARCHAR(50),
    Navegador VARCHAR(150)
);
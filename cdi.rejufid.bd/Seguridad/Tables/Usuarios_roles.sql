CREATE TABLE Usuarios_roles (
    Id_usuario_roles INT IDENTITY(1,1) PRIMARY KEY,
    Id_usuario VARCHAR(100) NOT NULL,
    Nombre_completo VARCHAR(100) NOT NULL,
    Matricula VARCHAR(50),
    Correo VARCHAR(150),
    Id_rol INT NOT NULL,
    Fecha_asignacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (Id_rol) REFERENCES Roles(Id_rol)
);